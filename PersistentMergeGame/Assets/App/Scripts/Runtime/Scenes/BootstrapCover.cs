// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;
using UnityEngine.UI;

namespace App.Scenes
{
    public class BootstrapCover : MonoBehaviour
    {
        [SerializeField] private Image image = null!;

        public void Hide()
        {
            image.enabled = false;
        }
    }
}
