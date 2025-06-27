using System;
using MergeGame.Core.Internal.Extensions;
using MergeGame.Core.Enums;
using MergeGame.Core.Internal.Managers;
using NUnit.Framework;
using UnityEngine;
using VContainer;

namespace MergeGame.Core.Tests
{
    public class GameSessionTest
    {
        private static IObjectResolver Build()
        {
            var bld = new ContainerBuilder();

            bld.Register<GameManager>(Lifetime.Singleton);

            return bld.Build();
        }

        private static GameManager GetSessionManager()
        {
            var resolver = Build();
            return resolver.Resolve<GameManager>();
        }

        [Test]
        public void CreateSession()
        {
            var sessionManager = GetSessionManager();
            var session = sessionManager.CreateGameSession(5, 5);
            Assert.IsNotNull(session);

            var result = sessionManager.GetSession(session.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);

            Assert.AreSame(session, result.Value);
            Debug.Log(result.Value);
        }

        [Test]
        public void PlaceBlock()
        {
            var manager = GetSessionManager();
            int width = 5;
            int height = 5;
            var session = manager.CreateGameSession(width, height);
            var board = manager.GetBoard(session);

            var pos1 = board.CreatePosition(0, 0);
            bool result = board.PlaceBlock(pos1, 0, BoardCellState.Untouchable);
            Assert.IsTrue(result, "Failed to place block at position (0, 0).");

            (int maxX, int maxY) = board.MaxPosition.AsPrimitive();
            var pos2 = board.CreatePosition(maxX, maxY);
            result = board.PlaceBlock(pos2, 1, BoardCellState.Untouchable);
            Assert.IsTrue(result, $"Failed to place block at position ({maxX}, {maxY}).");

            Assert.Throws<ArgumentOutOfRangeException>(() => { board.CreatePosition(-1, -1); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { board.CreatePosition(width, height); });
        }
    }
}
