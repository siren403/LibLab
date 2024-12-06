using VContainer.Unity;

namespace SceneLauncher
{
    public sealed record LaunchedContext
    {
        private LaunchedContext()
        {
        }

#if VCONTAINER
        public LifetimeScope Scope { get; private init; }
#endif

        internal static LaunchedContext FromOptions(LaunchOptions options)
        {
            return new LaunchedContext
            {
#if VCONTAINER
                Scope = options.Scope
#endif
            };
        }
    }
}