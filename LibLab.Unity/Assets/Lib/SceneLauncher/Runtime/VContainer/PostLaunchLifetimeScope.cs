#if VCONTAINER
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace SceneLauncher.VContainer
{
    public class PostLaunchLifetimeScope : LaunchedLifetimeScope
    {
        protected override void Awake()
        {
            autoRun = false;
            parentReference = new ParentReference();
            base.Awake();

            async UniTaskVoid Launch(CancellationToken cancellationToken)
            {
                // await UniTask.Yield(cancellationToken);
                var context = await StartupLauncher.LaunchedTask;
                Startup(context);
                // Debug.Log(gameObject.scene.name + " | " + nameof(Launch));
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