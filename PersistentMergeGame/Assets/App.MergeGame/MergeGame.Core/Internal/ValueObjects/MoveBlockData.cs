// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.ValueObjects;

internal struct MoveBlockData
{
    public Position FromPosition { get; init; }
    public Position ToPosition { get; init; }
}
