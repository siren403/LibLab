using UnityEngine.Assertions;
using VContainer.Unity;

namespace SceneLauncher
{
    public sealed class LaunchOptions
    {
        public LifetimeScope Scope { get; private set; }

        private LaunchOptions()
        {
        }
#if VCONTAINER
        public static LaunchOptions Create(LifetimeScope scope)
        {
            Assert.IsNotNull(scope);
            var options = new LaunchOptions
            {
                Scope = scope
            };
            return options;
        }
#else
        private static readonly LaunchOptions Default = new();

        public static LaunchOptions Create()
        {
            return Default;
        }
#endif
    }
}