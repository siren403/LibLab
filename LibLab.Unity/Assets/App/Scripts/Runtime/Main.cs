using App.Scenes;
using App.Scenes.Modal;
using SceneLauncher;
using SceneLauncher.VContainer;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace App
{
    public class SceneInstallerResolver
    {
        public IInstaller Resolve(Scene scene)
        {
            return scene switch
            {
                {buildIndex: 0} => new MainScene(),
                _ when scene.path.Contains("ModalScene") => new ModalScene(),
                _ => UnitInstaller.Instance
            };
        }
    }

    public class Main
    {
        private static readonly SceneInstallerResolver _sceneInstallerResolver = new();

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
            var installer = _sceneInstallerResolver.Resolve(scene);
            if (isMainScene)
            {
                ScopeInjector.CreateScope<StartupLifetimeScope>(
                    scene,
                    nameof(StartupLifetimeScope),
                    installer
                );
                SceneManager.SetActiveScene(scene);
            }
            else
            {
                ScopeInjector.CreateScope<PostLaunchLifetimeScope>(
                    scene,
                    nameof(PostLaunchLifetimeScope),
                    installer
                );
            }
        }
    }
}
