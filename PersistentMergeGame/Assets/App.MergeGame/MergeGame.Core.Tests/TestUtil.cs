// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.Internal.Entities;
using MergeGame.Core.Internal.Managers;
using VContainer;

namespace MergeGame.Core.Tests
{
    internal static class TestUtil
    {
        private static IObjectResolver Build()
        {
            var bld = new ContainerBuilder();

            bld.Register<GameManager>(Lifetime.Singleton);

            return bld.Build();
        }

        public static GameManager GetGameManager(out IObjectResolver resolver)
        {
            resolver = Build();
            return resolver.Resolve<GameManager>();
        }

        public static (GameSession, Board) CreateBoard(GameManager manager, int width, int height)
        {
            var session = manager.CreateGameSession(width, height);
            var board = manager.GetBoard(session);
            return (session, board);
        }
    }
}
