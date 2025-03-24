// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEngine;

namespace Storybook
{
    public interface IControl
    {
        public Component Source { get; }
        public Type SourceType { get; }
        public Type ValueType { get; }
        public ChangeResult OnValueChanged();
    }
}
