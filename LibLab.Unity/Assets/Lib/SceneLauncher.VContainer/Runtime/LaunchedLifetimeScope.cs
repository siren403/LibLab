using VContainer.Unity;

namespace SceneLauncher.VContainer
{
    public abstract class LaunchedLifetimeScope : LifetimeScope
    {
        public IInstaller? ExtraInstaller { set; protected get; }
    }
}
