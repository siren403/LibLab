// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using MergeGame.Common.Results;
using MergeGame.Core.Enums;
using MergeGame.Core.Internal.Entities;
using MergeGame.Core.Internal.Extensions;
using MergeGame.Core.Internal.Repositories;
using MergeGame.Core.Internal.ValueObjects;
using NUnit.Framework;
using UnityEngine;
using VContainer;

namespace MergeGame.Core.Tests
{
    public class BoardTest
    {
        [Test]
        [TestCase(5, 5)]
        public void PlaceBlock(int width, int height)
        {
            var manager = TestUtil.GetGameManager(out _);
            (_, Board board) = TestUtil.CreateBoard(manager, width, height);

            var pos1 = board.CreatePosition(0, 0);
            bool result = board.PlaceBlock(pos1, 0, BoardCellState.Untouchable);
            Assert.IsTrue(result, "Failed to place block at position (0, 0).");

            (int maxX, int maxY) = board.MaxPosition;
            var pos2 = board.CreatePosition(maxX, maxY);
            result = board.PlaceBlock(pos2, 1, BoardCellState.Untouchable);
            Assert.IsTrue(result, $"Failed to place block at position ({maxX}, {maxY}).");

            Assert.Throws<ArgumentOutOfRangeException>(() => { board.CreatePosition(-1, -1); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { board.CreatePosition(width, height); });
        }


        [Test]
        public void MergeBlock()
        {
            var manager = TestUtil.GetGameManager(out var container);
            (_, Board board) = TestUtil.CreateBoard(manager, 5, 5);

            var pos1 = board.CreatePosition(0, 0);
            var pos2 = board.CreatePosition(0, 1);

            bool placeResult = board.PlaceBlock(pos1, 0, BoardCellState.Movable);
            Assert.IsTrue(placeResult, "Failed to place block at position (0, 0).");
            placeResult = board.PlaceBlock(pos2, 0, BoardCellState.Movable);
            Assert.IsTrue(placeResult, "Failed to place block at position (0, 1).");

            var repository = container.Resolve<IMergeRuleRepository>();
            var mergeResult = board.MergeBlock(pos1, pos2, repository);
            Assert.IsTrue(mergeResult.IsOk);

            Debug.Log($"{mergeResult.Value}");
        }
    }
}
