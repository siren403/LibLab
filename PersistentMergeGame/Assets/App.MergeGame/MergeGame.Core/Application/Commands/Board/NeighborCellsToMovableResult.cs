// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.Application.Data;

namespace MergeGame.Core.Application.Commands.Board;

public readonly struct NeighborCellsToMovableResult
{
    public BoardCell[] UpdatedCells { get; init; }
}
