#if VCONTAINER

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SceneLauncher.VContainer.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLauncher
{
    public class SceneLoader : IProgress<float>
    {
        public static readonly SceneLoader Default = new();

        void IProgress<float>.Report(float value)
        {
            Debug.Log(value);
        }

        internal async UniTask AttachMainSceneAsync(SceneInstallers installers,
            CancellationToken cancellationToken = new())
        {
            // await UniTask.Yield(PlayerLoopTiming.PreUpdate);
            var operation = SceneManager.LoadSceneAsync(installers.MainScenePath, LoadSceneMode.Additive)!;
            await operation.ToUniTask(this, cancellationToken: cancellationToken);
            for (var i = 0; i < SceneManager.loadedSceneCount; i++)
            {
                var loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.path == installers.MainScenePath)
                {
                    SceneManager.SetActiveScene(loadedScene);
                    break;
                }
            }
        }

        public async UniTask AttachSceneAsync(string path, CancellationToken cancellationToken = new())
        {
            await UniTask.Yield(PlayerLoopTiming.PreUpdate, cancellationToken);
            var operation = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive)!;
            await operation.ToUniTask(this, cancellationToken: cancellationToken);
            // while (!operation.isDone)
            // {
            //     await UniTask.Yield(cancellation);
            // }
        }
    }
}
#endif