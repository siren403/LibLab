using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLauncher.Editor
{
    public static class TestModeInitializer
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"{scene.path} | {mode.ToString()}");
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var buildScene = SceneManager.GetSceneByBuildIndex(i);
                Debug.Log($"{i} {buildScene.path} | {buildScene.name}");
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            PrintSceneCount();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitialize()
        {
            PrintSceneCount();
        }

        private static void PrintSceneCount()
        {
            return;
            var count = SceneManager.sceneCountInBuildSettings;
            Debug.Log(count);
        }
    }
}