using MergeGame.Core.Enums;

namespace MergeGame.Core.ValueObjects
{
    public record BoardCellSpec(
        Position Position,
        BlockId BlockId,
        BoardCellState State
    );
}
