// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;

namespace Storybook.Controls
{
    [Serializable]
    public class SizeControl : ComponentControl<Sizes, Size>
    {
        public SizeControl() : base(Size.M)
        {

        }

        protected override ChangeResult OnValueChanged(Sizes source, Size value)
        {
            if (source.Size == value)
            {
                return ChangeResult.NotDirty;
            }
            return source.Apply(value) ? ChangeResult.Success : ChangeResult.NotDirty;
        }
    }

}
