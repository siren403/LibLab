#if VCONTAINER
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UnityEngine.SceneManagement;

namespace SceneLauncher.VContainer
{
    using Lazy = SceneLauncher.Internal.InitializableLazy<UniTaskCompletionSource<IObjectResolver>>;

    public class PostLaunchLifetimeScope : LaunchedLifetimeScope
    {
        private static readonly Dictionary<Scene, Lazy> _lazies = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _lazies.Clear();
        }

        public static UniTask<IObjectResolver> GetLaunchedTask(Scene scene)
        {
            if (_lazies.TryGetValue(scene, out Lazy lazy))
            {
                return lazy.Value.Task;
            }

            lazy = new Lazy(() => new UniTaskCompletionSource<IObjectResolver>());
            _lazies.Add(scene, lazy);
            return lazy.Value.Task;
        }

        protected override void Awake()
        {
            autoRun = false;
            parentReference = new ParentReference();
            base.Awake();
            GetLaunchedTask(gameObject.scene).Forget();

            async UniTaskVoid Launch(CancellationToken cancellationToken)
            {
                // await UniTask.Yield(cancellationToken);
                LaunchedContext context = await StartupLauncher.LaunchedTask;
                Startup(context);
                // Debug.Log(gameObject.scene.name + " | " + nameof(Launch));
                if (_lazies.TryGetValue(gameObject.scene, out Lazy lazy))
                {
                    lazy.Value.TrySetResult(Container);
                    Debug.Log("[Set Result] " + gameObject.scene.name);
                }
            }

            Launch(destroyCancellationToken).Forget();
        }

        private void Startup(LaunchedContext context)
        {
            if (ExtraInstaller != null)
            {
                using (Enqueue(ExtraInstaller))
                using (EnqueueParent(context.Scope))
                {
                    Build();
                }
            }
            else
            {
                using (EnqueueParent(context.Scope))
                {
                    Build();
                }
            }

            ExtraInstaller = null;
        }
    }
}
#endif
