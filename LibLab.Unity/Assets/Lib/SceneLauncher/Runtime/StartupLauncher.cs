using System;
using Cysharp.Threading.Tasks;
using SceneLauncher.Internal;
using UnityEngine;

namespace SceneLauncher
{
    public static class StartupLauncher
    {
        private static readonly InitializableLazy<UniTaskCompletionSource<LaunchedContext>> _launchedSource =
            new(() => new UniTaskCompletionSource<LaunchedContext>());

        private static bool _isExecutedLaunch;

        public static UniTask<LaunchedContext> LaunchedTask => _launchedSource.Value.Task;

        // Domain reload support
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _launchedSource.Initialize();
            _isExecutedLaunch = false;
        }

        public static void Launch(LaunchOptions options, Action configuration)
        {
            if (_isExecutedLaunch)
            {
                throw new InvalidOperationException("Already launched.");
            }

            _isExecutedLaunch = true;
            configuration();
            LaunchedContext context = LaunchedContext.FromOptions(options);
            _launchedSource.Value.TrySetResult(context);
        }
    }
}
