using App.Utils;
using Cysharp.Threading.Tasks;
using R3;
using SceneLauncher.VContainer;
using Storybook;
using Storybook.Buttons;
using UnityEngine;
using VContainer;

namespace App.Scenes.Modal
{
    public class AlertModal : ContainerBehaviour
    {
        [SerializeField] private ComponentSelector<PrimaryButton> closeButton;

        [Inject] private AlertState _state;

        protected override void OnBuild(IObjectResolver container)
        {
            _state.Message.AsObservable().Subscribe(closeButton.Source, (str, btn) =>
            {
                btn.Label = str;
            }).AddTo(this);
        }
    }
}
