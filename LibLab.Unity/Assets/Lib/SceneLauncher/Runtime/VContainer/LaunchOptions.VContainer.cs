#if VCONTAINER
using UnityEngine.Assertions;
using VContainer.Unity;

namespace SceneLauncher
{
    public sealed partial class LaunchOptions
    {
        public LifetimeScope Scope { get; private set; }

        public static LaunchOptions Create(LifetimeScope scope)
        {
            Assert.IsNotNull(scope);
            var options = new LaunchOptions
            {
                Scope = scope
            };
            return options;
        }
    }
}
#endif