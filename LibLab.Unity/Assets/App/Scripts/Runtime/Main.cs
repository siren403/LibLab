using System;
using System.Collections.Generic;
using System.IO;
using App.Features.LayeredBlocks;
using App.Scenes;
using App.Scenes.Modal;
using SceneLauncher;
using SceneLauncher.VContainer;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace App
{
    public class SceneInstallerResolver
    {
        public static SceneInstallerResolver Instance { get; } = new();

        private static readonly Dictionary<string, IInstaller> _installers = new();

        public IInstaller Resolve(Scene scene)
        {
            if (_installers.TryGetValue(scene.path, out var installer))
            {
                return installer;
            }

            return scene switch
            {
                { buildIndex: 0 } => new MainScene(),
                _ when scene.path.Contains("ModalScene") => new ModalScene(),
                _ when scene.path.Contains("LB_") => new LayerdBlocksScene(),
                _ => UnitInstaller.Instance
            };
        }

        public void Register(string path, IInstaller installer)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(path));
            Assert.IsNotNull(installer);
            _installers[path] = installer;
        }
    }

    public class Main
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterSceneLoaded()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CheckMainScene()
        {
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
            Debug.Log($"[{scene.buildIndex}] Scene loaded: {scene.path}");
            bool isMainScene = scene.buildIndex == 0;
            var installer = SceneInstallerResolver.Instance.Resolve(scene);
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
