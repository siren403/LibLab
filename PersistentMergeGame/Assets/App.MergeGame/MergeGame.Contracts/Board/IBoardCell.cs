// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace MergeGame.Contracts.Board
{
    public interface IBoardCell
    {
        int X { get; }
        int Y { get; }
        long BlockId { get; }
    }
}
