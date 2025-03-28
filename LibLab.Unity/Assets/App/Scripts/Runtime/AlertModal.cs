using Cysharp.Threading.Tasks;
using R3;
using SceneLauncher.VContainer;
using Storybook;
using Storybook.Buttons;
using UnityEngine;
using VContainer;

namespace App
{
    public class AlertModal : MonoBehaviour
    {
        [SerializeField] private ComponentSelector<PrimaryButton> closeButton;

        private void Awake()
        {
            if (closeButton.HasSource)
            {
                closeButton.Source.Button.OnClickAsObservable().Subscribe(_ =>
                {
                    if (!TryGetComponent(out Page page))
                    {
                        return;
                    }

                    page.Hide().Forget();
                }).AddTo(this);
            }

            PostLaunchLifetimeScope.GetLaunchedTask(gameObject.scene).ContinueWith((container) =>
            {
                if (container.TryResolve(out AlertState state))
                {
                    state.Message.AsObservable().Subscribe((str) =>
                    {
                        closeButton.Source.Label = str;
                    }).AddTo(this);
                }
            });
        }
    }
}
