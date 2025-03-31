using App.Scenes;
using App.Scenes.Modal;
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
        private static void RegisterSceneLoaded()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CheckMainScene()
        {
            bool loadedMainScene = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex != 0) continue;

                loadedMainScene = true;
                break;
            }

            if (!loadedMainScene)
            {
                SceneManager.LoadScene(0, LoadSceneMode.Additive);
            }
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
                SceneManager.SetActiveScene(scene);
            }
            else
            {
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
