// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.Enums;

namespace MergeGame.Core.ValueObjects
{
    public record BoardCellSpec(
        Position Position,
        BlockId BlockId,
        BoardCellState State
    );
}
