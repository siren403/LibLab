// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using MergeGame.Common.Results;
using MergeGame.Core.Enums;
using MergeGame.Core.Internal.Repositories;
using MergeGame.Core.Internal.ValueObjects;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Entities
{
    internal class Board
    {
        public Ulid Id { get; }
        public int Width => Cells.GetLength(0);
        public int Height => Cells.GetLength(1);
        private BoardCell[,] Cells { get; }

        public Position MaxPosition => new(Width - 1, Height - 1);

        private Board(Ulid id, BoardCell[,] cells)
        {
            Id = id;

            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells), "Cells cannot be null.");
            }

            if (cells.GetLength(0) == 0 || cells.GetLength(1) == 0)
            {
                throw new ArgumentException("Cells must have a non-zero size.", nameof(cells));
            }

            Cells = cells;
        }

        public Position CreatePosition(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(
                    $"Position ({x}, {y}) is out of bounds for the board size ({Width}, {Height}).");
            }

            return new Position(x, y);
        }

        public BoardCell GetCell(Position position)
        {
            (int x, int y) = position;
            return Cells[x, y];
        }

        public IEnumerable<BoardCell> GetCells()
        {
            return Cells.Cast<BoardCell>();
        }

        public Result<MergeBlockData> MergeBlock(Position from, Position to, IMergeRuleRepository repository)
        {
            var fromCell = GetCell(from);
            var toCell = GetCell(to);

            var canMerge = fromCell.CanMergeTo(toCell);

            if (canMerge.IsError<MergeBlockData>(out var canMergeFail))
            {
                return canMergeFail;
            }

            var fromBlockId = fromCell.BlockId!.Value;
            var mergeRuleResult = repository.FindMergeRule(fromBlockId);

            if (mergeRuleResult.IsError<MergeBlockData>(out var mergeRuleFail))
            {
                return mergeRuleFail;
            }

            var nextBlockId = mergeRuleResult.Value.NextBlockId;

            fromCell.RemoveBlock();
            toCell.RemoveBlock();
            toCell.PlaceBlock(nextBlockId, BoardCellState.Movable);

            return Result<MergeBlockData>.Ok(new MergeBlockData(
                fromCell,
                toCell
            ));
        }

        public Result<MoveBlockData> MoveBlock(Position from, Position to)
        {
            // 같은 위치 안됨
            if (from == to)
            {
                return Result<MoveBlockData>.Fail($"Cannot move block to the same position: {from}");
            }

            var fromCell = GetCell(from);
            var toCell = GetCell(to);

            // fromCell은 Movable 상태여야 하고
            if (fromCell.State != BoardCellState.Movable)
            {
                return Result<MoveBlockData>.Fail(
                    $"Cannot move block from {from} because it is not movable. State: {fromCell.State}");
            }

            if (toCell.HasBlock)
            {
                return Result<MoveBlockData>.Fail($"Cannot move block to {to} because it already has a block.");
            }

            var fromBlockId = fromCell.RemoveBlock();
            return toCell.PlaceBlock(fromBlockId, BoardCellState.Movable)
                ? Result<MoveBlockData>.Ok(new MoveBlockData() { FromPosition = from, ToPosition = to })
                : Result<MoveBlockData>.Fail($"Cannot place block at {to}. State: {toCell.State}");
        }

        public IEnumerable<BoardCell> GetNeighborCells(Position position)
        {
            int x = position.X;
            int y = position.Y;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue; // Skip the cell itself
                    int newX = x + dx;
                    int newY = y + dy;

                    if (newX >= 0 && newX < Width && newY >= 0 && newY < Height)
                    {
                        yield return GetCell(new Position(newX, newY));
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{nameof(Board)}({nameof(Id)}: {Id}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height})";
        }

        public static Board CreateWithCells(Ulid boardId, int width, int height)
        {
            BoardCell[,] cells = new BoardCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Position position = new(x, y);
                    cells[x, y] = BoardCell.CreateEmptyCell(boardId, position);
                }
            }

            return new Board(boardId, cells);
        }
    }
}
