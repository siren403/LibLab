// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using TMPro;

namespace Storybook.Controls
{
    [Serializable]
    public class LabelControl : ComponentControl<TextMeshProUGUI, string>
    {
        protected override ChangeResult OnValueChanged(TextMeshProUGUI source, string value)
        {
            if (source.text == value)
            {
                return ChangeResult.NotDirty;
            }
            source.text = value;
            return ChangeResult.Success;
        }
    }
}
