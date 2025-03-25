using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SceneLauncher;
using SceneLauncher.VContainer;
using Storybook.Buttons;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using VitalRouter;
using VitalRouter.VContainer;

namespace App
{
    public class ModalScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            Debug.Log("Installing: " + nameof(ModalScene));
            builder.RegisterInstance(GameObject.Find("AlertModal").GetComponent<RectTransform>());
            builder.RegisterEntryPoint<Init>();
            builder.RegisterComponentInHierarchy<PrimaryButton>();
            bool existsPerson = builder.Exists(
                typeof(IPerson),
                true,
                true);
            if (!existsPerson)
            {
                builder.Register<AlertPerson>(Lifetime.Singleton).As<IPerson>();
            }
            builder.RegisterVitalRouter((routing) =>
            {
                routing.Map<UiPresenter>();
            });
        }

        private class Init : IInitializable
        {
            private RectTransform _alert;
            private PrimaryButton _button;
            private IPerson _person;

            public Init(RectTransform alert, PrimaryButton button, IPerson person)
            {
                _alert = alert;
                _button = button;
                _person = person;
            }

            public void Initialize()
            {
                _alert.anchoredPosition = Vector2.zero;
                _button.Label = _person.Name;
            }
        }
    }

    internal interface IPerson
    {
        string Name { get; }
    }

    internal class MainPerson : IPerson
    {
        public string Name => "Main";
    }

    internal class AlertPerson : IPerson
    {
        public string Name => "Alert";
    }

    public partial struct ClickCommand : ICommand
    {
        public string Id { get; init; }
    }

    [Routes]
    public partial class UiPresenter
    {
        [Route(CommandOrdering.Drop)]
        private async ValueTask OnClick(ClickCommand command, CancellationToken cancellationToken)
        {
            Debug.Log("Clicked: " + command.Id);
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
        }
    }

    internal class MainScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            Debug.Log("Installing: " + builder.ApplicationOrigin);
            builder.Register<MainPerson>(Lifetime.Singleton).As<IPerson>();
        }
    }

    public class UnitInstaller : IInstaller
    {
        public static readonly UnitInstaller Instance = new();

        private UnitInstaller()
        {

        }

        public void Install(IContainerBuilder builder)
        {
        }
    }

    public class Main : MonoBehaviour
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

        private async UniTaskVoid Start()
        {
            await Addressables.InitializeAsync();
            await Addressables.CheckForCatalogUpdates();

            IList<IResourceLocation> locations =
                await Addressables.LoadResourceLocationsAsync("/").Task.AsUniTask();

            HashSet<string> loadedScenes = new();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Debug.Log("Loaded Scene: " + SceneManager.GetSceneAt(i).path);
                loadedScenes.Add(SceneManager.GetSceneAt(i).path);
            }

            List<AsyncOperationHandle<SceneInstance>> sceneHandles = new();
            List<UniTask> sceneLoadTasks = new();
            foreach (IResourceLocation location in locations)
            {
                if (loadedScenes.Contains(location.ToString()))
                {
                    Debug.Log("Scene already loaded: " + location);
                    continue;
                }
                string path = location.ToString();
                Debug.Log("Loading scene: " + path);
                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(location,
                    LoadSceneMode.Additive,
                    SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, false);
                handle.Destroyed += operationHandle =>
                {
                    Debug.Log("Scene destroyed: " + path);
                };
                sceneHandles.Add(handle);
                sceneLoadTasks.Add(handle.Task.AsUniTask());
            }

            await UniTask.WhenAll(sceneLoadTasks);

            foreach (AsyncOperationHandle<SceneInstance> sceneHandle in sceneHandles)
            {
                await sceneHandle.Result.ActivateAsync();
            }

            Debug.Log("All scenes loaded");

            // await UniTask.Delay(TimeSpan.FromSeconds(3));
            //
            // foreach (IResourceLocation location in locations)
            // {
            //     Debug.Log("Unloading scene: " + location);
            //     await SceneManager.UnloadSceneAsync(location.ToString());
            // }

            // foreach (AsyncOperationHandle<SceneInstance> sceneHandle in sceneHandles)
            // {
            //     sceneHandle.Release();
            // }
        }


    }
}
