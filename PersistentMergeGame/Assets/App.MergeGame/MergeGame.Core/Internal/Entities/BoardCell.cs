using System;
using MergeGame.Core.Enums;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Entities
{
    /// <summary>
    /// 블록 인스턴스의 상태 관리
    /// </summary>
    internal class BoardCell
    {
        public Ulid BoardId { get; private init; }
        public Position Position { get; private init; }
        public BlockId? BlockId { get; private set; }
        public BoardCellState State { get; private set; }

        public static BoardCell CreateEmptyCell(Ulid boardId, Position position)
        {
            return new BoardCell()
            {
                BoardId = boardId, Position = position, BlockId = null, State = BoardCellState.Untouchable,
            };
        }

        public bool ChangeMovable()
        {
            if (!HasBlock) return false;

            if (State != BoardCellState.Mergeable)
            {
                return false;
            }

            State = BoardCellState.Movable;
            return true;
        }

        public bool PlaceBlock(BlockId blockId, BoardCellState state)
        {
            if (HasBlock)
            {
                return false;
            }

            BlockId = blockId;
            State = state;
            return true;
        }

        public void RemoveBlock()
        {
            if (!HasBlock)
            {
                return;
            }

            BlockId = null;
            State = BoardCellState.Untouchable;
        }

        public bool HasBlock => BlockId.HasValue;

        public bool TryGetBlockId(out BlockId blockId)
        {
            if (HasBlock)
            {
                blockId = BlockId!.Value;
                return true;
            }

            blockId = default;
            return false;
        }

        public override string ToString()
        {
            return $"{nameof(BoardCell)}({nameof(BoardId)}: {BoardId}, {nameof(Position)}: {Position}, " +
                   $"{nameof(BlockId)}: {BlockId}, {nameof(State)}: {State})";
        }
    }
}
