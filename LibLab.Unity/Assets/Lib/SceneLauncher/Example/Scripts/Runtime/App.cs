using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace SceneLauncher.Example
{
    public static class App
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            var config = new StartupConfig
            {
                Aliases = new Dictionary<string, string>
                {
                    {"$scenes", "Assets/Lib/SceneLauncher/Example/Scenes"}
                },
                MainScene = new MainScene("$scenes/MainScene.unity"),
                SubScenes = new ISceneInstaller[]
                {
                    new ScenePathInstaller("$scenes/Scene A.unity"),
                    new ScenePathInstaller("$scenes/Scene B.unity"),
                    new ScenePathInstaller("$scenes/Scene C.unity")
                }
            };
            StartupLauncher.Initialize(config);
        }

        public class ScenePathInstaller : ISceneInstaller
        {
            public ScenePathInstaller(string path)
            {
                ScenePath = path;
            }

            public string ScenePath { get; }

            public virtual void Install(IContainerBuilder builder)
            {
            }
        }

        public sealed class MainScene : ScenePathInstaller
        {
            public MainScene(string path) : base(path)
            {
            }

            public override void Install(IContainerBuilder builder)
            {
                builder.RegisterEntryPoint<Program>();
            }

            private class Program : IInitializable
            {
                void IInitializable.Initialize()
                {
                    Debug.Log("MainScene | Initialized");
                    SceneManager.LoadScene("Lib/SceneLauncher/Example/Scenes/Scene A", LoadSceneMode.Additive);
                }
            }
        }
    }
}