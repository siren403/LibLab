namespace MergeGame.Contracts.Board
{
    public interface IBoardCell
    {
        int X { get; }
        int Y { get; }
        long BlockId { get; }
        short CellState { get; }
    }
}
