// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using VitalRouter;
using ZLogger;

namespace App.Navigation
{
    public class SceneNavigatorBuilder
    {
        private readonly IContainerBuilder _builder;

        public bool StartupRoot = true;

        public SceneNavigatorBuilder(IContainerBuilder builder)
        {
            _builder = builder;
        }
    }

    public static class SceneNavigatorExtensions
    {
        private static readonly AsyncLazy _readyCache = new(async () =>
        {
            await UniTask.WaitUntil(() => Caching.ready);
        });

        public static void RegisterSceneNavigator(this IContainerBuilder builder,
            Action<SceneNavigatorBuilder> configure)
        {
            var nav = new SceneNavigatorBuilder(builder);
            configure(nav);

            builder.RegisterInstance(new SceneNavigatorOptions()
            {
                StartupRoot = nav.StartupRoot,
            });
            builder.Register<SceneNavigator>(Lifetime.Singleton).AsSelf();
            builder.RegisterEntryPoint<SceneNavigatorInitializer>();
        }

        public static void StartupRootOnlyMainScene(this SceneNavigatorBuilder builder)
        {
            builder.StartupRoot = SceneManager.GetSceneAt(0).buildIndex == 0;
        }

        public static async UniTask CheckForUpdates(this SceneNavigator navigator)
        {
            var updates = await AddressableExtensions.CheckForCatalogUpdates();
            if (!updates.IsSuccess)
            {
                return;
            }

            if (!updates.Result.Any())
            {
                return;
            }

            // TODO: AddressableExtensions.UpdateCatalogs
            {
                await _readyCache;
                await Addressables.UpdateCatalogs(true, updates.Result);
            }

            await navigator.Clear();
            await navigator.Startup();
        }
    }

    public record SceneNavigatorOptions
    {
        public readonly string Root = "/";
        public bool StartupRoot { get; init; } = true;
    }

    public class SceneNavigatorInitializer : IAsyncStartable
    {
        private readonly SceneNavigator _navigator;

        public SceneNavigatorInitializer(SceneNavigator navigator)
        {
            _navigator = navigator;
        }

        public UniTask StartAsync(CancellationToken cancellation = default)
        {
            return _navigator.Initialize();
        }
    }

    public class SceneNavigator
    {
        private readonly SceneNavigatorOptions _options;
        private readonly Router _router;
        private readonly ILogger<SceneNavigator> _logger;

        private readonly Stack<string> _history = new();
        private readonly Dictionary<string, IList<IResourceLocation>> _locationCache = new();
        private readonly HashSet<string> _loadedScenesCache = new();
        private readonly List<AsyncOperationHandle<SceneInstance>> _loadingSceneHandles = new();

        private bool _initialized;
        public bool IsInitialized => _initialized;

        public string? LoadedLocation => _history.Any() ? _history.Peek() : null;

        public SceneNavigator(SceneNavigatorOptions options, Router router, ILogger<SceneNavigator> logger)
        {
            _options = options;
            _router = router;
            _logger = logger;
        }

        public async UniTask Initialize()
        {
            var initialized = await AddressableExtensions.Initialize();

            if (!initialized.IsSuccess)
            {
                _logger.LogError("Failed to initialize");
                _ = _router.PublishAsync(new InitializeFailedCommand());
                return;
            }

            _logger.LogInformation("Initialized");
            _ = _router.PublishAsync(new InitializedCommand()
            {
                Keys = initialized.Result.Keys.ToArray()
            });

            _initialized = true;

            if (_options.StartupRoot)
            {
                await Startup();
            }
        }

        public async UniTask Startup()
        {
            await To(_options.Root);
        }

        public async UniTask To(string path)
        {
            Assert.IsTrue(_initialized);
            IList<IResourceLocation> locations = await GetLocations(path);
            if (locations.Count == 0)
            {
                _logger.ZLogWarning($"Empty resource locations: {path}");
                return;
            }

            if (_history.Any())
            {
                string top = _history.Peek()!;
                if (top != _options.Root)
                {
                    await UnloadRoute(top);
                }
                else
                {
                    _logger.LogWarning("Impossible unload root path");
                }
            }

            await LoadRoute(path);
            _history.Push(path);
        }

        public async UniTask Back()
        {
            if (_history.Count <= 1)
            {
                _logger.LogWarning("Back failed. empty history");
                return;
            }

            string top = _history.Pop()!;
            await UnloadRoute(top);

            string peeked = _history.Peek()!;
            if (peeked == _options.Root)
            {
                _logger.LogWarning("Impossible reload root path");
                return;
            }

            await LoadRoute(peeked);
        }

        public async UniTask Clear()
        {
            if (_history.Count == 0)
            {
                return;
            }

            string top = _history.Pop()!;
            await UnloadRoute(top);

            _history.Clear();
            _locationCache.Clear();
            _loadedScenesCache.Clear();
            _loadingSceneHandles.Clear();
            _logger.LogInformation("Clear");
        }

        private async UniTask<IList<IResourceLocation>> GetLocations(string path)
        {
            if (!_locationCache.TryGetValue(path, out var locations))
            {
                locations = await Addressables.LoadResourceLocationsAsync(path).Task;
                _locationCache.Add(path, locations);
            }

            return locations;
        }

        private async UniTask UnloadRoute(string path)
        {
            var locations = await GetLocations(path);

            if (locations.Count == 0)
            {
                _logger.ZLogError($"Failed to unload resource locations: {path}");
                return;
            }

            _ = _router.PublishAsync(new PreUnloadRouteCommand()
            {
                Path = path
            });

            GetLoadedScenes(_loadedScenesCache);

            foreach (var location in locations)
            {
                string locationString = location.ToString();
                if (!_loadedScenesCache.Contains(locationString))
                {
                    _logger.ZLogWarning($"Not loaded {locationString} from {path}");
                    continue;
                }

                var operation = SceneManager.UnloadSceneAsync(locationString);
                if (operation == null)
                {
                    _logger.ZLogError($"Unload {locationString} operation is null.");
                    continue;
                }

                await operation;
            }

            _logger.ZLogInformation($"Unloaded {path}");
        }

        private async UniTask LoadRoute(string path)
        {
            var locations = await GetLocations(path);

            if (locations.Count == 0)
            {
                _logger.ZLogError($"Failed to load resource locations: {path}");
                return;
            }

            _ = _router.PublishAsync(new PreLoadRouteCommand()
            {
                Path = path
            });

            GetLoadedScenes(_loadedScenesCache);

            _loadingSceneHandles.Clear();

            foreach (IResourceLocation location in locations)
            {
                string locationString = location.ToString();
                if (_loadedScenesCache.Contains(locationString))
                {
                    _logger.ZLogWarning($"Loaded {locationString} from {path}");
                    continue;
                }

                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(location,
                    LoadSceneMode.Additive,
                    SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, false);

                if (!handle.IsValid())
                {
                    _logger.ZLogError($"Invalid handle {locationString} from {path}");
                    continue;
                }

                await handle.Task;

                _loadingSceneHandles.Add(handle);
            }

            if (_loadingSceneHandles.Count == 0)
            {
                _logger.ZLogWarning($"Failed to load resource locations: {path}");
                return;
            }

            foreach (AsyncOperationHandle<SceneInstance> handle in _loadingSceneHandles)
            {
                await handle.Result.ActivateAsync();
            }

            _loadingSceneHandles.Clear();
            _logger.ZLogInformation($"Loaded {path}");
        }

        private void GetLoadedScenes(in HashSet<string> cache)
        {
            cache.Clear();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                cache.Add(SceneManager.GetSceneAt(i).path);
            }
        }
    }

    public partial struct PreUnloadRouteCommand : ICommand
    {
        public string Path { get; init; }
    }

    public partial struct PreLoadRouteCommand : ICommand
    {
        public string Path { get; init; }
    }

    public partial struct InitializedCommand : ICommand
    {
        public object[] Keys { get; init; }
    }

    public partial struct InitializeFailedCommand : ICommand
    {
    }
}
