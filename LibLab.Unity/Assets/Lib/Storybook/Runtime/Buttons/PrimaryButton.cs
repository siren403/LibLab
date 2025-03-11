using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Storybook.Buttons
{
    public enum Sizes
    {
        Small,
        Medium,
        Large
    }

    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class PrimaryButton : MonoBehaviour
    {
        private readonly ComponentCache<Button> _button = new();
        private readonly ComponentCache<TextMeshProUGUI> _label = new(CacheLocation.Children);
        private readonly ComponentCache<RectTransform> _rect = new();

        [SerializeField]
        private ComponentSelector<Button> button;

        public bool Disabled
        {
            get => !_button.Get(gameObject).interactable;
            set
            {
                _button.Get(gameObject).interactable = !value;
                _label.Get(gameObject).color = !value ? Color.white : Color.gray;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(_button.Get(gameObject));
                UnityEditor.EditorUtility.SetDirty(_label.Get(gameObject));
#endif
            }
        }

        public string Label
        {
            get => _label.Get(gameObject).text;
            set
            {
                _label.Get(gameObject).text = value;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(_label.Get(gameObject));
#endif
            }
        }

        [SerializeField] [HideInInspector]
        private Sizes _size = Sizes.Medium;

        public Sizes Size
        {
            get => _size;
            set
            {
                _size = value;
                RectTransform rect = _rect.Get(gameObject);
                TextMeshProUGUI label = _label.Get(gameObject);
                Vector2 mediumRect = new()
                {
                    x = 374, y = 145
                };
                float mediumFontSize = 54;
                switch (_size)
                {
                    case Sizes.Small:
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mediumRect.x * 0.8f);
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mediumRect.y * 0.8f);
                        if (label.enableAutoSizing)
                        {
                            label.fontSizeMax = mediumFontSize * 0.8f;
                        }
                        else
                        {
                            label.fontSize = mediumFontSize * 0.8f;
                        }
                        break;
                    case Sizes.Medium:
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mediumRect.x);
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mediumRect.y);
                        if (label.enableAutoSizing)
                        {
                            label.fontSizeMax = mediumFontSize;
                        }
                        else
                        {
                            label.fontSize = mediumFontSize;
                        }
                        break;
                    case Sizes.Large:
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mediumRect.x * 1.2f);
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mediumRect.y * 1.2f);
                        if (label.enableAutoSizing)
                        {
                            label.fontSizeMax = mediumFontSize * 1.2f;
                        }
                        else
                        {
                            label.fontSize = mediumFontSize * 1.2f;
                        }
                        break;
                }
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(rect);
                UnityEditor.EditorUtility.SetDirty(label);
#endif
            }
        }


        private void Reset()
        {
            Disabled = false;
            Label = "Button";
            Size = Sizes.Medium;
        }
    }
}
