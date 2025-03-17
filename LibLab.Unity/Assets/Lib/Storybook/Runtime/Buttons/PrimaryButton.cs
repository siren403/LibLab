using Storybook.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace Storybook.Buttons
{

    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class PrimaryButton : MonoBehaviour
    {
        [SerializeField]
        private ComponentSelector<Button> button;

        [SerializeField]
        private DisabledControl disabled;

        [SerializeField]
        private LabelControl label;

        [SerializeField]
        private ButtonSizeControl size;

        public string Label
        {
            set => label.Value = value;
        }
    }
}
