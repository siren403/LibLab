// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Contracts.Board;

namespace MergeGame.Core.Application.Data
{
    public record BoardCell(
        int X,
        int Y,
        long BlockId
    ) : IBoardCell;
}
