using App.Scenes.Modal;
using SceneLauncher.VContainer;
using UnityEngine;

namespace App
{
    public static class Main
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            var resolver = new SceneInstallerResolver();
            resolver.Register("Assets/App/Scenes/App_MainScene.unity",
                new Scenes.MainScene()
            );
            resolver.Register("Assets/App/Features/UI/Scenes/App_UI_ModalScene.unity",
                new ModalScene()
            );
            resolver.Register(
                "Assets/App/Scenes/App_NavigatorScene.unity",
                new Scenes.NavigatorScene()
            );
            resolver.Register(
                "Assets/App/Features/UI/Scenes/App_UI_TransitionScene.unity",
                new Scenes.UI.TransitionScene()
            );
            SceneScopeInitializer.Initialize(resolver);
        }
    }
}
