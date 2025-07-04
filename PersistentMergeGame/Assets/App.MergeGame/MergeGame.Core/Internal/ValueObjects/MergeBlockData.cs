// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.Internal.Entities;

namespace MergeGame.Core.Internal.ValueObjects
{
    internal record MergeBlockData(
        BoardCell FromCell,
        BoardCell ToCell
    );
}
