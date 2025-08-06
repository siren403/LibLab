// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VExtensions.SceneNavigation.Commands;
using VExtensions.SceneNavigation.Extensions;
using VitalRouter;
using ZLogger;

namespace VExtensions.SceneNavigation
{
    public class Navigator
    {
        private readonly NavigatorOptions _options;
        private readonly Router _router;
        private readonly ILogger<Navigator> _logger;

        private readonly Stack<string> _history = new();
        private readonly Dictionary<string, IList<IResourceLocation>> _locationCache = new();
        private readonly HashSet<string> _loadedScenesCache = new();
        private readonly List<AsyncOperationHandle<SceneInstance>> _loadingSceneHandles = new();

        private bool _initialized;
        public bool IsInitialized => _initialized;

        public string LoadedLocation { get; private set; } = string.Empty;

        public bool HasHistory => _history.Count > 1;

        public Navigator(NavigatorOptions options, Router router, ILogger<Navigator> logger)
        {
            _options = options;
            _router = router;
            _logger = logger;
        }

        public async UniTask Initialize()
        {
            if (_initialized)
            {
                throw new InvalidOperationException("Already initialized.");
            }

            var initialized = await AddressableExtensions.Initialize();

            if (!initialized.IsSuccess)
            {
                _logger.LogError("Failed to initialize");
                _ = _router.PublishAsync(new InitializeFailedCommand());
                return;
            }

            _logger.LogInformation("Initialized");
            _ = _router.PublishAsync(new InitializedCommand() { Keys = initialized.Result.Keys.ToArray() });

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

            if (!string.IsNullOrEmpty(LoadedLocation))
            {
                if (LoadedLocation != _options.Root)
                {
                    await UnloadRoute(LoadedLocation);
                    LoadedLocation = string.Empty;
                }
                else
                {
                    _logger.LogWarning("Impossible unload root path");
                }
            }

            await LoadRoute(path);
            LoadedLocation = path;
        }

        public async UniTask Push(string path)
        {
            Assert.IsTrue(_initialized);
            await To(path);
            _history.Push(path);
        }

        public async UniTask Back()
        {
            if (!HasHistory)
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
            LoadedLocation = peeked;
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

            _ = _router.PublishAsync(new PreUnloadRouteCommand() { Path = path });

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

            _ = _router.PublishAsync(new PreLoadRouteCommand() { Path = path });

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
            _ = _router.PublishAsync(new PostLoadRouteCommand() { Path = path });
            LoadedLocation = path;
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
}
