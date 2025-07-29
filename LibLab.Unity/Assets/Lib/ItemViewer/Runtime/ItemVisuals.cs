// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;

namespace ItemViewer
{
    [Serializable]
    public abstract class ItemVisuals
    {
        /// <summary>
        /// The containing <see cref="ItemView"/>.
        /// </summary>
        [field: NonSerialized] public NullOr<ItemView> View { get; internal set; }
    }
}
