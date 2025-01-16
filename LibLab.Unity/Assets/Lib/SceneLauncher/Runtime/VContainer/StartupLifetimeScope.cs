#if VCONTAINER
using VContainer.Unity;

namespace SceneLauncher.VContainer
{
    public class StartupLifetimeScope : LaunchedLifetimeScope
    {
        protected override void Awake()
        {
            autoRun = false;
            parentReference = new ParentReference();
            base.Awake();

            var options = LaunchOptions.Create(this);
            StartupLauncher.Launch(options, () =>
            {
                if (ExtraInstaller != null)
                {
                    using (Enqueue(ExtraInstaller))
                    {
                        Build();
                    }
                }
                else
                {
                    Build();
                }

                ExtraInstaller = null;
            });
        }
    }
}
#endif