// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;

namespace Storybook.Controls
{
    [Serializable]
    public class ButtonSizeControl : ComponentControl<ButtonSizes, Size>
    {
        public ButtonSizeControl() : base(Size.M)
        {

        }

        protected override ChangeResult OnValueChanged(ButtonSizes source, Size value)
        {
            if (source.Size == value)
            {
                return ChangeResult.NotDirty;
            }
            return source.Apply(value) ? ChangeResult.Success : ChangeResult.NotDirty;
        }
    }
}
