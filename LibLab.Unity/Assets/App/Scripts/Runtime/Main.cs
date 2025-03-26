using SceneLauncher;
using SceneLauncher.VContainer;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace App
{
    public class Main
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[{scene.buildIndex}] Scene loaded: {scene.path}");
            bool isMainScene = scene.buildIndex == 0;
            if (isMainScene)
            {
                ScopeInjector.CreateScope<StartupLifetimeScope>(
                    scene,
                    nameof(StartupLifetimeScope),
                    new MainScene()
                );
            }
            else
            {
                Debug.Log($"MainScene +> {scene.path}");
                IInstaller installer = scene.path switch
                {
                    _ when scene.path.Contains("ModalScene") => new ModalScene(),
                    _ => UnitInstaller.Instance
                };
                ScopeInjector.CreateScope<PostLaunchLifetimeScope>(
                    scene,
                    nameof(PostLaunchLifetimeScope),
                    installer
                );
            }
        }

    }
}
