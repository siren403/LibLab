// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace App
{
    public class AddressableRouterEntry : IAsyncStartable
    {
        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            await Addressables.InitializeAsync();
            await Addressables.CheckForCatalogUpdates();

            IList<IResourceLocation> locations =
                await Addressables.LoadResourceLocationsAsync("/").Task.AsUniTask();

            HashSet<string> loadedScenes = new();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Debug.Log("Loaded Scene: " + SceneManager.GetSceneAt(i).path);
                loadedScenes.Add(SceneManager.GetSceneAt(i).path);
            }

            List<AsyncOperationHandle<SceneInstance>> sceneHandles = new();
            List<UniTask> sceneLoadTasks = new();
            foreach (IResourceLocation location in locations)
            {
                if (loadedScenes.Contains(location.ToString()))
                {
                    Debug.Log("Scene already loaded: " + location);
                    continue;
                }
                string path = location.ToString();
                Debug.Log("Loading scene: " + path);
                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(location,
                    LoadSceneMode.Additive,
                    SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, false);
                handle.Destroyed += operationHandle =>
                {
                    Debug.Log("Scene destroyed: " + path);
                };
                sceneHandles.Add(handle);
                sceneLoadTasks.Add(handle.Task.AsUniTask());
            }

            await UniTask.WhenAll(sceneLoadTasks);

            foreach (AsyncOperationHandle<SceneInstance> sceneHandle in sceneHandles)
            {
                await sceneHandle.Result.ActivateAsync();
            }

            Debug.Log("All scenes loaded");
        }
    }
}
