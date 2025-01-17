#if VCONTAINER
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SceneLauncher.VContainer;
using SceneLauncher.VContainer.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLauncher
{
    public static partial class StartupLauncher
    {
        private static readonly SceneInstallers Installers = new();
        private static readonly ScenePathParser ScenePathParser = new();

        public static IAliases Aliases => ScenePathParser;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistration()
        {
            Installers.Clear();
            ScenePathParser.Aliases.Clear();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public static void Initialize(StartupConfig config)
        {
#if UNITY_INCLUDE_TESTS
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.path.Contains("TestScene"))
            {
                Debug.LogWarning("Test mode is enabled.");
                return;
            }
#endif
            foreach (var alias in config.Aliases)
            {
                ScenePathParser.Aliases[alias.Key] = alias.Value;
            }

            Installers.Add(config.MainScene, ScenePathParser, SceneInstallers.AddMode.Main);

            foreach (var installer in config.SubScenes)
            {
                Installers.Add(installer, ScenePathParser, SceneInstallers.AddMode.Sub);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var loadedScene = scene.path;

            if (Installers.TryGetValue(loadedScene, out var installer, out var addMode))
            {
                switch (addMode)
                {
                    case SceneInstallers.AddMode.Main:
                        ScopeInjector.CreateScope<StartupLifetimeScope>(
                            scene,
                            nameof(StartupLifetimeScope),
                            installer
                        );
                        break;
                    default:
                        ScopeInjector.CreateScope<PostLaunchLifetimeScope>(
                            scene,
                            nameof(PostLaunchLifetimeScope),
                            installer);
                        break;
                }
            }

            if (!IsLoadedMainScene(out var path))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Main scene is not loaded.: {path}");
#endif
                SceneLoader.Default.AttachMainSceneAsync(Installers).Forget();
            }

            bool IsLoadedMainScene(out string result)
            {
                result = null;
                var mainScenePath = Installers.MainScenePath;
                for (var i = 0; i < SceneManager.loadedSceneCount; i++)
                {
                    if (mainScenePath == SceneManager.GetSceneAt(i).path)
                    {
                        return true;
                    }
                }

                result = mainScenePath;
                return false;
            }
        }
    }

    public record StartupConfig
    {
        public IReadOnlyDictionary<string, string> Aliases { get; init; } = new Dictionary<string, string>();
        public ISceneInstaller MainScene { get; init; }
        public IReadOnlyCollection<ISceneInstaller> SubScenes { get; init; } = ArraySegment<ISceneInstaller>.Empty;
    }
}
#endif