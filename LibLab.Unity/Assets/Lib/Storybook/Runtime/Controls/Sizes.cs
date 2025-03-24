// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEngine;

namespace Storybook.Controls
{
    [RequireComponent(typeof(RectTransform))]
    public class Sizes : MonoBehaviour
    {
        [SerializeField]
        private Vector2 referenceSize = new(100, 100);

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
            if (!TryGetComponent(out RectTransform rectTransform))
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
                Vector2 targetSize = referenceSize * preset.scale;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetSize.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetSize.y);
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
