// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.ValueObjects;

internal record MoveResult(
    bool Ok,
    Position FromPosition,
    Position ToPosition
)
{
    public static readonly MoveResult Error = new MoveResult(false, default, default);
}
