using System;
using MergeGame.Common.Results;
using MergeGame.Core.Enums;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Entities
{
    /// <summary>
    /// 블록 인스턴스의 상태 관리
    /// </summary>
    internal class BoardCell
    {
        public static BoardCell CreateEmptyCell(Ulid boardId, Position position)
        {
            return new BoardCell()
            {
                BoardId = boardId, Position = position, BlockId = null, State = BoardCellState.Untouchable,
            };
        }

        public Ulid BoardId { get; private init; }
        public Position Position { get; private init; }
        public BlockId? BlockId { get; private set; }
        public BoardCellState State { get; private set; }

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
            if (!blockId.IsValid)
            {
                return false;
            }

            if (HasBlock)
            {
                return false;
            }

            BlockId = blockId;
            State = state;
            return true;
        }

        public BlockId RemoveBlock()
        {
            var wasId = BlockId ?? Core.ValueObjects.BlockId.Invalid;
            BlockId = null;
            State = BoardCellState.Untouchable;
            return wasId;
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

        public Result CanMergeTo(BoardCell target)
        {
            if (State != BoardCellState.Movable)
            {
                return Result.Fail(ErrorCode.CannotMergeBlock, $"{nameof(BoardCell)} is not movable. State: {State}");
            }

            if (target.State == BoardCellState.Untouchable)
            {
                return Result.Fail(ErrorCode.CannotMergeBlock,
                    $"{nameof(BoardCell)} is untouchable. State: {target.State}");
            }

            if (Position == target.Position)
            {
                return Result.Fail(ErrorCode.CannotMergeBlock,
                    $"{nameof(BoardCell)} cannot merge with itself. Position: {Position}");
            }

            if (!TryGetBlockId(out var blockId) || !target.TryGetBlockId(out var targetBlockId))
            {
                return Result.Fail(ErrorCode.CannotMergeBlock,
                    $"{nameof(BoardCell)} does not have a valid BlockId. " +
                    $"BlockId: {BlockId}, TargetBlockId: {target.BlockId}");
            }

            if (blockId != targetBlockId)
            {
                return Result.Fail(ErrorCode.CannotMergeBlock,
                    $"{nameof(BoardCell)} cannot merge with different BlockIds. " +
                    $"BlockId: {blockId}, TargetBlockId: {targetBlockId}");
            }

            return Result.Ok();
        }

        public override string ToString()
        {
            return $"{nameof(BoardCell)}({nameof(BoardId)}: {BoardId}, {nameof(Position)}: {Position}, " +
                   $"{nameof(BlockId)}: {BlockId}, {nameof(State)}: {State})";
        }
    }
}
