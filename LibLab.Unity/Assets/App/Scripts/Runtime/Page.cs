// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace App
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class Page : MonoBehaviour
    {
        private Canvas _canvas;
        private RectTransform _transform;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _transform = GetComponent<RectTransform>();

            _canvas.enabled = false;
            _transform.anchoredPosition = Vector2.zero;
        }

        private void Reset()
        {
            var canvas = GetComponent<Canvas>();
            canvas.vertexColorAlwaysGammaSpace = true;

            var tr = GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;

            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;
        }
    }
}
