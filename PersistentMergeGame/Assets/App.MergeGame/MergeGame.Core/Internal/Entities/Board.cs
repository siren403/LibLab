// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MergeGame.Core.Enums;
using MergeGame.Core.Internal.ValueObjects;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Entities
{
    internal class Board
    {
        public Ulid Id { get; init; }
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

        public Result<MergeResult> MergeBlock(Position from, Position to)
        {
            // 같은 위치 안됨
            if (from == to)
            {
                return new Error<MergeResult>($"Cannot merge a cell with itself at position {from}.");
            }

            var fromCell = GetCell(from);
            var toCell = GetCell(to);

            // fromCell, toCell 둘 다 비어있으면 안됨
            if (!fromCell.TryGetBlockId(out var fromBlockId) || !toCell.TryGetBlockId(out var toBlockId))
            {
                return new Error<MergeResult>($"Cannot merge empty cells at positions {from} and {to}.");
            }

            // fromCell, toCell 둘 다 같은 블록 ID여야 함
            if (fromBlockId != toBlockId)
            {
                return new Error<MergeResult>(
                    $"Cannot merge cells with different block IDs: {fromBlockId} and {toBlockId} at positions {from} and {to}.");
            }

            // fromCell은 Movable 상태여야 하고
            if (fromCell.State != BoardCellState.Movable)
            {
                return new Error<MergeResult>(
                    $"Cannot merge from cell at {from} because it is not in a movable state. Current state: {fromCell.State}.");
            }

            // toCell은 Untouchable 상태면 안됨
            if (toCell.State == BoardCellState.Untouchable)
            {
                return new Error<MergeResult>(
                    $"Cannot merge to cell at {to} because it is in an untouchable state. Current state: {toCell.State}.");
            }

            fromCell.RemoveBlock();
            toCell.RemoveBlock();

            // TODO: 다음 블록 ID 생성 로직을 추가해야 함
            // 나중에는 머지 가능한 블록 ID인지 검증 부분이 필요함
            BlockId newBlockId = fromBlockId + 1;
            toCell.PlaceBlock(newBlockId, BoardCellState.Movable);

            return new Ok<MergeResult>(new MergeResult(
                fromCell,
                toCell
            ));
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
