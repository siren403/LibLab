// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
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

        public static Board CreateWithCells(Ulid boardId, int width, int height)
        {
            BoardCell[,] cells = new BoardCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Ulid cellId = Ulid.NewUlid();
                    Position position = new(x, y);
                    cells[x, y] = BoardCell.CreateEmptyCell(cellId, boardId, position);
                }
            }

            return new Board(boardId, cells);
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
            (int x, int y) = position.AsPrimitive();
            return Cells[x, y];
        }

        public override string ToString()
        {
            return $"{nameof(Board)}({nameof(Id)}: {Id}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height})";
        }
    }
}
