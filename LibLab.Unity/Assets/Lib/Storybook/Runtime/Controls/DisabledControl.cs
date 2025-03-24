// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEngine.UI;

namespace Storybook.Controls
{
    [Serializable]
    public class DisabledControl : ComponentControl<Button, bool>
    {
        protected override ChangeResult OnValueChanged(Button source, bool value)
        {
            bool toValue = !value;
            if (source.interactable == toValue)
            {
                return ChangeResult.NotDirty;
            }
            source.interactable = toValue;
            return ChangeResult.Success;
        }
    }
}
