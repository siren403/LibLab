using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter;

namespace App.Scenes.UI
{
    public class TransitionScene : IInstaller
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            SceneInstallerResolver.Instance.Register(
                "Assets/App/Features/UI/Scenes/App_UI_TransitionScene.unity",
                new TransitionScene()
            );
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CanvasGroup>();
            builder.RegisterEntryPoint<FadeController>();
        }

        public enum FadeMode
        {
            In,
            Out
        }

        public readonly struct FadeCommand : ICommand
        {
            public readonly FadeMode Mode;
            public readonly float Duration;

            private FadeCommand(FadeMode mode, float duration)
            {
                Mode = mode;
                Duration = duration;
            }

            public static FadeCommand In(float duration = 0.3f) => new(FadeMode.In, duration);
            public static FadeCommand Out(float duration = 0.3f) => new(FadeMode.Out, duration);
        }

        private class FadeController : IInitializable, IDisposable
        {
            private readonly CanvasGroup _fade;
            private readonly Router _router;
            private DisposableBag _disposable;

            public FadeController(CanvasGroup fade, Router router)
            {
                _fade = fade;
                _router = router;
            }

            public void Initialize()
            {
                _router.SubscribeAwait<FadeCommand>(async (cmd, ctx) =>
                {
                    float from = cmd.Mode == FadeMode.In ? 0f : 1f;
                    float to = cmd.Mode == FadeMode.In ? 1f : 0f;
                    var cancellationToken = ctx.CancellationToken;

                    await LMotion.Create(from, to, cmd.Duration)
                        .Bind(t => _fade.alpha = t)
                        .ToUniTask(cancellationToken: cancellationToken);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: cancellationToken);
                }, CommandOrdering.Switch).AddTo(ref _disposable);
            }

            public void Dispose()
            {
                _disposable.Dispose();
            }
        }
    }
}
