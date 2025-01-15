using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SceneLauncher.Example
{
    public class LifetimeLogger : MonoBehaviour
    {
        [SerializeField] private int updateCount = 1;

        private int _ticks;

        private void Awake()
        {
            Debug.Log($"{name} | {nameof(Awake)}");
            StartupLauncher.LaunchedTask.ContinueWith(_ => { Debug.Log($"{name} | Launched"); });
        }

        private void Start()
        {
            Debug.Log($"{name} | {nameof(Start)}");
        }

        private void Update()
        {
            _ticks++;
            if (_ticks <= updateCount)
            {
                Debug.Log($"{name} | {nameof(Update)} | {_ticks}");
            }
        }

        private void OnEnable()
        {
            Debug.Log($"{name} | {nameof(OnEnable)}");
        }

        private void OnDisable()
        {
            Debug.Log($"{name} | {nameof(OnDisable)}");
        }

        private void OnDestroy()
        {
            Debug.Log($"{name} | {nameof(OnDestroy)}");
        }
    }
}