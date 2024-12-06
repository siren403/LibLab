using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SceneLauncher
{
    public static class StartupLauncher
    {
        private static readonly InitializableLazy<UniTaskCompletionSource<LaunchedContext>> LaunchedSource =
            new(() => new UniTaskCompletionSource<LaunchedContext>());

        private static bool _isExecutedLaunch;

        public static UniTask<LaunchedContext> LaunchedTask => LaunchedSource.Value.Task;

        // Domain reload support
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            LaunchedSource.Initialize();
            _isExecutedLaunch = false;
        }

        public static void Launch(LaunchOptions options, Action configuration)
        {
            if (_isExecutedLaunch) throw new InvalidOperationException("Already launched.");

            _isExecutedLaunch = true;
            var cancellationToken = Application.exitCancellationToken;
            UniTask.Yield(PlayerLoopTiming.PreUpdate, cancellationToken).ContinueWith(() =>
            {
                configuration();
                var context = LaunchedContext.FromOptions(options);
                LaunchedSource.Value.TrySetResult(context);
            });
        }
    }
}