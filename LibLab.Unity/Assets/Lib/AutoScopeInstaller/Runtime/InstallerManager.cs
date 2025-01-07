using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace AutoScopeInstaller
{
    public static class InstallerManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            if (TaggedScenes.Instance == null)
            {
                Debug.Log("Not created TaggedScenes. Please 'Assets/Create/Auto Scope Installer/Tagged Scenes'.");
                return;
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var taggedScenes = TaggedScenes.Instance.Scenes;

            var findPath = taggedScenes.FirstOrDefault(reference => reference.Tag == "Dummy")?.Path ?? string.Empty;
            if (findPath == scene.path)
            {
                Debug.Log($"Find path by tag: {scene.name}");
            }

            var scope = LifetimeScope.Create(builder => { Debug.Log("CreateScope"); });
            SceneManager.MoveGameObjectToScene(scope.gameObject, scene);
        }
    }
}