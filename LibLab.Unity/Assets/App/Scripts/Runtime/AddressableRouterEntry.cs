// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer.Unity;
using ZLogger;

namespace App
{
    public class AddressableRouterEntry : IAsyncStartable
    {
        private readonly ILogger<AddressableRouterEntry> _logger;

        public AddressableRouterEntry(ILogger<AddressableRouterEntry> logger)
        {
            _logger = logger;
        }

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var initResult = await AddressableExtensions.Initialize();
                _logger.ZLogInformation($"Addressable Initialized: {initResult.Result}");
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    foreach (IResourceLocation location in initResult.Result.AllLocations)
                    {
                        _logger.ZLogDebug($"Addressable Location: {location}");
                    }
                }

                // Try enter root route
                var firstScene = SceneManager.GetSceneAt(0);
                if (firstScene.buildIndex != 0)
                {
                    return;
                }

                // Catalog updates

                await Push("/");

                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
                await AddressableExtensions.CheckForCatalogUpdates();
                await Push("/intro");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        private async UniTask Push(string path)
        {
            IList<IResourceLocation> locations =
                await Addressables.LoadResourceLocationsAsync(path).Task;

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

            await UniTask.WhenAll(sceneLoadTasks);

            foreach (AsyncOperationHandle<SceneInstance> sceneHandle in sceneHandles)
            {
                await sceneHandle.Result.ActivateAsync();
            }

            _logger.ZLogInformation($"Load completed: {path}");
        }
    }
}
