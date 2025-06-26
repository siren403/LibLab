// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using App.MergeGame.Core.Enums;
using MergeGame.Core.Enums;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Entities
{
    /// <summary>
    /// 블록 인스턴스의 상태 관리
    /// </summary>
    internal class BoardCell
    {
        public Ulid Id { get; private init; }
        public Ulid BoardId { get; private init; }
        public Position Position { get; private init; }
        public BlockId? BlockId { get; private set; }
        public BoardCellState State { get; private set; }

        public bool CanMove { get; private set; }
        public bool CanMerge { get; private set; }

        public bool IsSelectable => State == BoardCellState.Occupied && CanMove && CanMerge;

        public static BoardCell CreateEmptyCell(Ulid id, Ulid boardId, Position position)
        {
            return new BoardCell()
            {
                Id = id,
                BoardId = boardId,
                Position = position,
                BlockId = null,
                State = BoardCellState.Empty,
                CanMove = false,
                CanMerge = false
            };
        }

        public bool PlaceBlock(BlockId blockId, PlaceBlockType type)
        {
            if (State != BoardCellState.Empty)
            {
                return false;
            }

            BlockId = blockId;
            State = BoardCellState.Occupied;

            switch (type)
            {
                case PlaceBlockType.Untouchable:
                    CanMove = false;
                    CanMerge = false;
                    break;
                case PlaceBlockType.Mergeable:
                    CanMove = false;
                    CanMerge = true;
                    break;
                case PlaceBlockType.Movable:
                    CanMove = true;
                    CanMerge = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return true;
        }
    }
}
