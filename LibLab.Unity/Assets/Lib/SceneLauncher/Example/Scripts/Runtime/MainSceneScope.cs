using SceneLauncher;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SceneLauncher.Example
{
    public class MainSceneScope : LifetimeScope
    {
        protected override void Awake()
        {
            autoRun = false;
            parentReference = new ParentReference();
            base.Awake();

            var options = LaunchOptions.Create(this);
            StartupLauncher.Launch(options, () =>
            {
                Debug.Log("MainSceneDeps | Initialized");
                Build();
            });
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Program>();
            builder.Register<Shared>(Lifetime.Singleton);
            builder.RegisterBuildCallback(resolver =>
            {
                Debug.Log("CreateScope");
                var scoped = resolver.CreateScope(child =>
                {
                    child.RegisterEntryPoint<ChildProgram>();
                    child.RegisterEntryPoint<ChildProgram2>();
                });
            });
        }

        private class ChildProgram : IInitializable
        {
            public ChildProgram(Shared shared)
            {
                Debug.Log($"ChildProgram | {shared.Value} | {shared.GetHashCode()}");
            }

            public void Initialize()
            {
            }
        }

        private class ChildProgram2 : IInitializable
        {
            public ChildProgram2(Shared shared)
            {
                Debug.Log($"ChildProgram2 | {shared.Value} | {shared.GetHashCode()}");
            }

            public void Initialize()
            {
            }
        }

        private class Program : IInitializable, IStartable
        {
            public Program(Shared shared)
            {
                Debug.Log($"MainSceneScope | Constructed | {shared.Value} | {shared.GetHashCode()}");
            }

            public void Initialize()
            {
                Debug.Log("MainSceneScope | Initialized");
            }

            public void Start()
            {
                Debug.Log("MainSceneScope | Started");
            }
        }
    }

    public sealed class Shared
    {
        public string Value = "Shared";

        public Shared()
        {
            Debug.Log($"Shared | Constructed | {GetHashCode()}");
        }
    }
}
