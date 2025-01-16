#if VCONTAINER
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SceneLauncher.VContainer;
using SceneLauncher.VContainer.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

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
            return;
            if (SceneLauncher.TestMode)
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
                        CreateScope<StartupLifetimeScope>(nameof(StartupLifetimeScope), installer);
                        break;
                    default:
                        CreateScope<PostLaunchLifetimeScope>(nameof(PostLaunchLifetimeScope), installer);
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

            void CreateScope<T>(string name, IInstaller extraInstaller) where T : LaunchedLifetimeScope
            {
                var gameObject = new GameObject(name ?? "LifetimeScope");
                gameObject.SetActive(false);
                var newScope = gameObject.AddComponent<T>();
                newScope.ExtraInstaller = extraInstaller;
                SceneManager.MoveGameObjectToScene(gameObject, scene);
                gameObject.SetActive(true);
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