using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App.Scenes
{
    public class NavigatorScene : IInstaller
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            SceneInstallerResolver.Instance.Register(
                "Assets/App/Scenes/App_NavigatorScene.unity",
                new NavigatorScene()
            );
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<AddressableComponents>();
            builder.RegisterEntryPoint<AddressableRouterEntry>();
        }
    }
}
