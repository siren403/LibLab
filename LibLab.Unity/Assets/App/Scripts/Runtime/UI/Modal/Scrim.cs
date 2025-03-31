// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Storybook;
using UnityEngine;
using UnityEngine.UI;

namespace App.UI.Modal
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Button))]
    public class Scrim : MonoBehaviour
    {
        [SerializeField] private ComponentSelector<CanvasGroup> canvasGroup;
        [SerializeField] private ComponentSelector<Button> button;

        public CanvasGroup CanvasGroup => canvasGroup;
        public Button Button => button;
    }
}
