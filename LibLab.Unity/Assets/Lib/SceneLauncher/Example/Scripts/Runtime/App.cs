using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
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
                SubScenes = new[]
                {
                    new SceneActionInstaller("$scenes/Scene A.unity",
                        builder => { EntryPointLogger.Register<SceneALogger>(builder, "Scene A", 1); }),
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

        public sealed class SceneActionInstaller : ScenePathInstaller
        {
            private readonly Action<IContainerBuilder> _action;

            public SceneActionInstaller(string path, Action<IContainerBuilder> action) : base(path)
            {
                Assert.IsNotNull(action);
                _action = action;
            }

            public override void Install(IContainerBuilder builder)
            {
                _action(builder);
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
                EntryPointLogger.Register<MainLogger>(builder, "Main", 0);
            }

            private class Program : IInitializable
            {
                void IInitializable.Initialize()
                {
                    if (SceneManager.loadedSceneCount == 1)
                    {
                        SceneLoader.Default.AttachSceneAsync("Lib/SceneLauncher/Example/Scenes/Scene A").Forget();
                    }
                }
            }
        }

        private class MainLogger : EntryPointLogger
        {
            public MainLogger(string key, int updateCount) : base(key, updateCount)
            {
            }
        }

        private class SceneALogger : EntryPointLogger
        {
            public SceneALogger(string key, int updateCount) : base(key, updateCount)
            {
            }
        }

        private partial class EntryPointLogger
        {
            public static void Register<T>(IContainerBuilder builder, string key, int updateCount)
                where T : EntryPointLogger
            {
                builder.RegisterEntryPoint<T>()
                    .WithParameter(key)
                    .WithParameter(updateCount);
            }
        }

        private partial class EntryPointLogger : IInitializable, IStartable, IAsyncStartable, ITickable
        {
            private readonly string _key;
            private readonly int _updateCount;

            private int _ticks;

            public EntryPointLogger(string key, int updateCount)
            {
                _key = key;
                _updateCount = updateCount;
            }

            public UniTask StartAsync(CancellationToken cancellation = new())
            {
                Debug.Log($"{_key} | EntryPoint | {nameof(StartAsync)}");
                return UniTask.CompletedTask;
            }

            public void Initialize()
            {
                Debug.Log($"{_key} ({GetHashCode()}) | EntryPoint | {nameof(Initialize)}");
            }

            public void Start()
            {
                Debug.Log($"{_key} | EntryPoint | {nameof(Start)}");
            }

            public void Tick()
            {
                _ticks++;
                if (_ticks <= _updateCount)
                {
                    Debug.Log($"{_key} | EntryPoint | {nameof(Tick)}");
                }
            }
        }
    }
}