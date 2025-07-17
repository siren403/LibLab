// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using GameKit.Common.Results;
using MergeGame.Core.Enums;
using MergeGame.Core.Internal.Repositories;
using MergeGame.Core.Internal.ValueObjects;
using MergeGame.Core.ValueObjects;
using Unity.Mathematics;
using ZLinq;
using ZLinq.Linq;

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

        public ValueEnumerable<FromNonGenericEnumerable<BoardCell>, BoardCell> GetCells()
        {
            return Cells.AsValueEnumerable<BoardCell>();
        }

        public FastResult<MergeBlockData> MergeBlock(Position from, Position to, IMergeRuleRepository repository)
        {
            var fromCell = GetCell(from);
            var toCell = GetCell(to);

            var canMerge = fromCell.CanMergeTo(toCell);

            if (canMerge.IsError(out FastResult<MergeBlockData> canMergeFail))
            {
                return canMergeFail;
            }

            var fromBlockId = fromCell.BlockId!.Value;
            var mergeRuleResult = repository.FindMergeRule(fromBlockId);

            if (mergeRuleResult.IsError(out FastResult<MergeBlockData> mergeRuleFail))
            {
                return mergeRuleFail;
            }

            var nextBlockId = mergeRuleResult.Value.NextBlockId;

            fromCell.RemoveBlock();
            toCell.RemoveBlock();
            toCell.PlaceBlock(nextBlockId, BoardCellState.Movable);

            return FastResult<MergeBlockData>.Ok(new MergeBlockData(
                fromCell,
                toCell
            ));
        }

        public FastResult<MoveBlockData> MoveBlock(Position from, Position to)
        {
            // 같은 위치 안됨
            if (from == to)
            {
                return FastResult<MoveBlockData>.Fail($"Cannot move block to the same position: {from}");
            }

            var fromCell = GetCell(from);
            var toCell = GetCell(to);

            // fromCell은 Movable 상태여야 하고
            if (fromCell.State != BoardCellState.Movable)
            {
                return FastResult<MoveBlockData>.Fail(
                    $"Cannot move block from {from} because it is not movable. State: {fromCell.State}");
            }

            if (toCell.HasBlock)
            {
                return FastResult<MoveBlockData>.Fail($"Cannot move block to {to} because it already has a block.");
            }

            var fromBlockId = fromCell.RemoveBlock();
            return toCell.PlaceBlock(fromBlockId, BoardCellState.Movable)
                ? FastResult<MoveBlockData>.Ok(new MoveBlockData() { FromPosition = from, ToPosition = to })
                : FastResult<MoveBlockData>.Fail($"Cannot place block at {to}. State: {toCell.State}");
        }

        /// <summary>
        /// 주변 1칸의 Cell들을 가져옵니다.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
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

        public FastResult<BoardCell> FindNearestEmptyCell(Position from, Position to)
        {
            var nearestEmptyCell = GetCells()
                .Where(c => !c.HasBlock)
                .Append(GetCell(from))
                .OrderBy(c => Position.DistanceSq(c.Position, to))
                .FirstOrDefault();

            return nearestEmptyCell != null
                ? FastResult<BoardCell>.Ok(nearestEmptyCell)
                : FastResult<BoardCell>.Fail("Board.FindNearestEmptyCell",
                    $"No empty cell found near position {to}.");
        }

        /// <summary>
        /// 방향 기반으로 보드 경계 위치를 계산한 후 가장 가까운 빈 셀을 찾습니다.
        /// </summary>
        /// <param name="from">시작 위치</param>
        /// <param name="direction">방향 벡터</param>
        /// <returns>방향 끝에서 가장 가까운 빈 셀</returns>
        public FastResult<BoardCell> FindNearestEmptyCellFromDirection(Position from, Direction direction)
        {
            if (!direction.IsValid)
            {
                // 방향이 유효하지 않으면 원래 위치 반환
                return FastResult<BoardCell>.Ok(GetCell(from));
            }

            // 방향 * 보드 사이즈로 충분히 멀리 이동
            var targetX = from.X + direction.X * Width;
            var targetY = from.Y + direction.Y * Height;

            // 보드 경계로 클램핑하여 경계 위치 계산
            var boundaryPosition = new Position(
                math.clamp((int)math.round(targetX), 0, Width - 1),
                math.clamp((int)math.round(targetY), 0, Height - 1)
            );

            // 기존 FindNearestEmptyCell 로직 재사용
            return FindNearestEmptyCell(from, boundaryPosition);
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
