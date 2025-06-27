using MergeGame.Contracts.Board;

namespace MergeGame.Core.Application.Data
{
    public record BoardCell(
        int X,
        int Y,
        long BlockId
    ) : IBoardCell
    {
        internal static BoardCell FromEntity(MergeGame.Core.Internal.Entities.BoardCell entity)
        {
            return new BoardCell(
                entity.Position.AsPrimitive().x,
                entity.Position.AsPrimitive().y,
                entity.BlockId?.AsPrimitive() ?? -1
            );
        }
    }
}
