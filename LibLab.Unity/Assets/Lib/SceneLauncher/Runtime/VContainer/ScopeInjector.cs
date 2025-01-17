#if VCONTAINER

using SceneLauncher.VContainer;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace SceneLauncher
{
    public static class ScopeInjector
    {
        public static T CreateScope<T>(Scene scene, string name, IInstaller extraInstaller)
            where T : LaunchedLifetimeScope
        {
            var gameObject = new GameObject(name ?? "LifetimeScope");
            gameObject.SetActive(false);
            var newScope = gameObject.AddComponent<T>();
            newScope.ExtraInstaller = extraInstaller;
            SceneManager.MoveGameObjectToScene(gameObject, scene);
            gameObject.SetActive(true);
            return newScope;
        }
    }
}
#endif