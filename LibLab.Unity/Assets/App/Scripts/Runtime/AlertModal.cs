using Cysharp.Threading.Tasks;
using R3;
using Storybook;
using Storybook.Buttons;
using UnityEngine;

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
        }
    }
}
