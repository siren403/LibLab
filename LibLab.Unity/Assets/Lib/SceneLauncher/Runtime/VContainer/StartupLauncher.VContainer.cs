#if VCONTAINER
using System;
using System.Collections.Generic;
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
                        using (LifetimeScope.Enqueue(installer))
                        {
                            CreateScope<StartupLifetimeScope>(nameof(StartupLifetimeScope))
                                .ExtraInstaller = installer;
                        }

                        break;
                    default:
                        using (LifetimeScope.Enqueue(installer))
                        {
                            CreateScope<PostLaunchLifetimeScope>(nameof(PostLaunchLifetimeScope))
                                .ExtraInstaller = installer;
                        }

                        break;
                }
            }

            T CreateScope<T>(string name) where T : LifetimeScope
            {
                using (LifetimeScope.Enqueue(installer))
                {
                    var gameObject = new GameObject(name ?? "LifetimeScope");
                    gameObject.SetActive(false);
                    var newScope = gameObject.AddComponent<T>();
                    gameObject.SetActive(true);
                    return newScope;
                }
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