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
            resolver.Register("Assets/App/Scenes/App_MainScene.unity", new Scenes.MainScene());
            resolver.Register("Assets/App/Scenes/App_MergeGameScene.unity", new Scenes.MergeGame.MergeGameScene());
            SceneScopeInitializer.Initialize(resolver);
        }
    }
}
