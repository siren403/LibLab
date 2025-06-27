// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.Enums;
using MergeGame.Core.Internal.Entities;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Extensions
{
    internal static class BoardExtensions
    {
        public static bool PlaceBlock(this Board board, Position position, BlockId blockId, BoardCellState state)
        {
            var cell = board.GetCell(position);
            return cell.PlaceBlock(blockId, state);
        }
    }
}
