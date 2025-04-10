// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Scenes
{
    public class AddressableComponents : MonoBehaviour
    {
        [field: SerializeField] public Button? CheckUpdateButton { get; private set; }
        [field: SerializeField] public Button? UpdateButton { get; private set; }
        [field: SerializeField] public Button? PushButton { get; private set; }
        [field: SerializeField] public Button? PopButton { get; private set; }
        [field: SerializeField] public TextMeshProUGUI? LogLabel { get; private set; }
        [field: SerializeField] public ScreenFadeFeature? ScreenFadeFeature { get; private set; }
    }
}
