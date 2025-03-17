// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using TMPro;
using UnityEngine;

namespace Storybook.Controls
{
    public class ButtonSizes : MonoBehaviour
    {
        [SerializeField]
        private ComponentSelector<RectTransform> rect;

        [SerializeField]
        private ComponentSelector<TextMeshProUGUI> text;

        [SerializeField]
        private Vector2 referenceRectSize = new(100, 100);

        [SerializeField]
        private float referenceFontSize = 32;

        [SerializeField]
        private Preset[] presets =
        {
            new()
            {
                size = Size.S, scale = 0.8f
            },
            new()
            {
                size = Size.M, scale = 1f
            },
            new()
            {
                size = Size.L, scale = 1.2f
            }
        };

        [SerializeField] [HideInInspector]
        private Size currentSize = Size.M;

        public Size Size => currentSize;

        public bool Apply(Size size)
        {
            if (!rect.HasSource || !text.HasSource)
            {
                return false;
            }

            currentSize = size;
            foreach (Preset preset in presets)
            {
                if (preset.size != currentSize)
                {
                    continue;
                }
                Vector2 targetRectSize = referenceRectSize * preset.scale;
                rect.Source.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetRectSize.x);
                rect.Source.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetRectSize.y);

                float targetFontSize = referenceFontSize * preset.scale;
                if (text.Source.enableAutoSizing)
                {
                    text.Source.fontSizeMax = targetFontSize;
                }
                else
                {
                    text.Source.fontSize = targetFontSize;
                }
                return true;
            }
            return false;
        }

        [Serializable]
        public struct Preset
        {
            public Size size;
            public float scale;
        }
    }
}
