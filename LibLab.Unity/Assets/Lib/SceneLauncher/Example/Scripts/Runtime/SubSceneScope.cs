using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SceneLauncher.Example
{
    public class SubSceneScope : LifetimeScope
    {
        protected override void Awake()
        {
            autoRun = false;
            parentReference = new ParentReference();
            base.Awake();
            StartupLauncher.LaunchedTask.ContinueWith(Startup);
            Debug.Log("SubSceneScope | Awake");
        }

        private void Startup(LaunchedContext context)
        {
            using (EnqueueParent(context.Scope))
            {
                Debug.Log("SubSceneScope | Build");
                Build();
            }
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Program>();
        }

        private class Program : IInitializable, IStartable
        {
            public Program(Shared shared)
            {
                Debug.Log($"SubSceneScope | Constructed | {shared.Value} | {shared.GetHashCode()}");
            }

            public void Initialize()
            {
                Debug.Log("SubSceneScope | Initialized");
            }

            public void Start()
            {
                Debug.Log("SubSceneScope | Started");
            }
        }
    }
}