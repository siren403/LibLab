#if VCONTAINER
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace SceneLauncher.VContainer
{
    public class PostLaunchLifetimeScope : LifetimeScope
    {
        internal IInstaller ExtraInstaller;

        protected override void Awake()
        {
            autoRun = false;
            parentReference = new ParentReference();
            base.Awake();
            StartupLauncher.LaunchedTask.ContinueWith(Startup);
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