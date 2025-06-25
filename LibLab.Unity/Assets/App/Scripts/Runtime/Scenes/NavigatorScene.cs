using VContainer;
using VContainer.Unity;

namespace App.Scenes
{
    public class NavigatorScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<AddressableComponents>();
            builder.RegisterEntryPoint<AddressableRouterEntry>();
        }
    }
}
