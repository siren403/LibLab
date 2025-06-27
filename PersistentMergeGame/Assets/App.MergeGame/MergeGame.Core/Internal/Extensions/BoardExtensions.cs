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
