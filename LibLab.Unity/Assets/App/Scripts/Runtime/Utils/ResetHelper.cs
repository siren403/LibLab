// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;

namespace App.Utils
{
    public static class ResetHelper
    {
        public static void ResetCanvas(this GameObject go)
        {
            Canvas canvas = go.GetComponent<Canvas>();
            canvas.vertexColorAlwaysGammaSpace = true;
        }

        public static void ResetRectTransform(this GameObject go)
        {
            RectTransform tr = go.GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;

            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;
        }
    }
}
