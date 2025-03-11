using UnityEngine;
using UnityEngine.UI;

namespace Storybook.Controls.Buttons
{
    [RequireComponent(typeof(Button))]
    public class Disabled : Controls.Disabled
    {
        private readonly ComponentCache<Button> _button = new();

        public override bool Value
        {
            get => !_button.Get(gameObject).interactable;
            set
            {
                _button.Get(gameObject).interactable = !value;
                Dirty(_button.Get(gameObject));
            }
        }
    }
}
