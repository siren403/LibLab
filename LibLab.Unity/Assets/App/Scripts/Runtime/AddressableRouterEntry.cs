// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using App.Scenes;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer.Unity;
using ZLogger;

namespace App
{
    public class AddressableRouterEntry : IAsyncStartable, IDisposable
    {
        private readonly ILogger<AddressableRouterEntry> _logger;
        private readonly AddressableComponents _components;
        private DisposableBag _disposables;

        public AddressableRouterEntry(ILogger<AddressableRouterEntry> logger, AddressableComponents components)
        {
            _logger = logger;
            _components = components;

            AsyncLazy readyCache = new(async () => { await UniTask.WaitUntil(() => Caching.ready); });
            List<string> catalogs = new List<string>();


            components.CheckUpdateButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha1)))
                .SubscribeAwait(async (_, cancellationToken) =>
                {
                    components.LogLabel!.text = nameof(AddressableExtensions.CheckForCatalogUpdates);
                    var updates = await AddressableExtensions.CheckForCatalogUpdates();
                    catalogs.Clear();
                    if (updates.Status == AsyncOperationStatus.Succeeded)
                    {
                        catalogs.AddRange(updates.Result);
                        components.LogLabel!.text = $"{catalogs.Count} catalogs loaded.";
                    }
                    else
                    {
                        components.LogLabel!.text = updates.OperationException.Message;
                    }
                }, AwaitOperation.Drop)
                .AddTo(ref _disposables);
            components.UpdateButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha2)))
                .SubscribeAwait(async (_, cancellationToken) =>
                {
                    components.LogLabel!.text = nameof(Addressables.UpdateCatalogs);
                    await readyCache;
                    if (catalogs.Count == 0)
                    {
                        _logger.LogWarning("Empty catalog list.");
                        return;
                    }

                    await Addressables.UpdateCatalogs(true, catalogs);
                    components.LogLabel!.text = $"{catalogs.Count} catalogs updated.";
                    catalogs.Clear();
                }, AwaitOperation.Drop)
                .AddTo(ref _disposables);

            components.PushButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha3)))
                .SubscribeAwait(async (_, cancellationToken) => { await Push("/intro"); }, AwaitOperation.Drop)
                .AddTo(ref _disposables);
            components.PopButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha4)))
                .SubscribeAwait(async (_, cancellationToken) => { await Pop(); },
                    AwaitOperation.Drop)
                .AddTo(ref _disposables);
        }

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            var initialized = await AddressableExtensions.Initialize();
            if (initialized.Status == AsyncOperationStatus.Failed)
            {
                _logger.LogError("Failed to Initialize Addressables.");
                return;
            }

            _components.LogLabel!.text = $"Initialized Addressables. {initialized.Result.Keys.Count()}";
            _logger.ZLogInformation($"Addressable Initialized: {initialized.Result.Keys.Count()}");

            // Try enter root route
            var firstScene = SceneManager.GetSceneAt(0);
            if (firstScene.buildIndex != 0)
            {
                return;
            }

            await Push("/");
        }

        private readonly Stack<string> _history = new();

        private async UniTask Push(string path)
        {
            if (_history.Count == 0 && path != "/")
            {
                _logger.ZLogError($"Path {path} has not been initialized.");
                return;
            }

            IList<IResourceLocation> locations =
                await Addressables.LoadResourceLocationsAsync(path).Task;

            if (locations.Count == 0)
            {
                _logger.ZLogError($"Failed to load resource locations: {path}");
                return;
            }

            if (_history.Contains(path))
            {
                _logger.ZLogWarning($"Already pushed: {path}");
                return;
            }

            _history.Push(path);

            HashSet<string> loadedScenes = new();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).path);
            }

            List<AsyncOperationHandle<SceneInstance>> sceneHandles = new();
            List<UniTask> sceneLoadTasks = new();
            foreach (IResourceLocation location in locations)
            {
                if (loadedScenes.Contains(location.ToString()))
                {
                    _logger.ZLogDebug($"Scene already loaded: {location}");
                    continue;
                }

                string locationStr = location.ToString();
                _logger.ZLogDebug($"Loading scene: {locationStr}");
                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(location,
                    LoadSceneMode.Additive,
                    SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, false);
                // handle.Destroyed += operationHandle => { Debug.Log("Scene destroyed: " + locationStr); };
                sceneHandles.Add(handle);
                sceneLoadTasks.Add(handle.ToUniTask());
            }

            if (sceneLoadTasks.Count == 0)
            {
                _logger.ZLogError($"Nothing to load scenes: {path}");
                return;
            }

            await UniTask.WhenAll(sceneLoadTasks);

            foreach (AsyncOperationHandle<SceneInstance> sceneHandle in sceneHandles)
            {
                await sceneHandle.Result.ActivateAsync();
            }

            _logger.ZLogInformation($"Load completed: {path}");
        }

        public async UniTask Pop()
        {
            if (_history.Count == 1)
            {
                _logger.LogWarning("Only the root path remains");
                return;
            }

            string path = _history.Pop();
            IList<IResourceLocation> locations =
                await Addressables.LoadResourceLocationsAsync(path).Task;

            HashSet<string> loadedScenes = new();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).path);
            }

            List<UniTask> sceneUnloadTasks = new();
            foreach (IResourceLocation location in locations)
            {
                if (!loadedScenes.Contains(location.ToString()))
                {
                    _logger.ZLogWarning($"Try unloading {location} scene. but not loaded");
                    continue;
                }

                var operation = SceneManager.UnloadSceneAsync(location.ToString());
                if (operation == null)
                {
                    _logger.ZLogError($"Failed to unload operation {location} scene");
                    continue;
                }

                sceneUnloadTasks.Add(operation.ToUniTask());
            }

            if (sceneUnloadTasks.Count == 0)
            {
                _logger.ZLogError($"Nothing to unload: {path}");
                return;
            }

            await UniTask.WhenAll(sceneUnloadTasks);

            _logger.ZLogInformation($"Unload completed: {path}");
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
