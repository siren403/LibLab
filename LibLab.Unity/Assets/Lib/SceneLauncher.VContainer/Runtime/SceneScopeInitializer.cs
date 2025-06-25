﻿// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLauncher.VContainer
{
    public static class SceneScopeInitializer
    {
        private static SceneInstallerResolver? _resolver;

        public static void Initialize(SceneInstallerResolver? resolver)
        {
            _resolver = resolver;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterSceneLoaded()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_resolver != null)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CheckMainScene()
        {
            if (_resolver == null)
            {
                return;
            }

            bool loadedMainScene = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex != 0) continue;

                loadedMainScene = true;
                break;
            }

            if (!loadedMainScene)
            {
                SceneManager.LoadScene(0, LoadSceneMode.Additive);
            }
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_resolver == null)
            {
                return;
            }

            bool isMainScene = scene.buildIndex == 0;
            var installer = _resolver.Resolve(scene);
            if (isMainScene)
            {
                ScopeInjector.CreateScope<StartupLifetimeScope>(
                    scene,
                    nameof(StartupLifetimeScope),
                    installer
                );
                SceneManager.SetActiveScene(scene);
            }
            else
            {
                ScopeInjector.CreateScope<PostLaunchLifetimeScope>(
                    scene,
                    nameof(PostLaunchLifetimeScope),
                    installer
                );
            }
        }
    }
}
