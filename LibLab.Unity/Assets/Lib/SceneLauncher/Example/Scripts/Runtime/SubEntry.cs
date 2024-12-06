using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLauncher.Example
{
    public class SubEntry : MonoBehaviour
    {
        private void Awake()
        {
            UniTask.WaitForSeconds(1, cancellationToken: destroyCancellationToken).ContinueWith(() =>
            {
                SceneManager.LoadScene("Lib/SceneLauncher/Example/Scenes/MainScene", LoadSceneMode.Additive);
                SceneManager.LoadScene("Lib/SceneLauncher/Example/Scenes/SubScene 1", LoadSceneMode.Additive);
            });
        }
    }
}