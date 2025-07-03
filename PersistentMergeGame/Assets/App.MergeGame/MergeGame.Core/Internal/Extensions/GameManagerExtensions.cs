// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using MergeGame.Core.Internal.Entities;
using MergeGame.Core.Internal.Managers;

namespace MergeGame.Core.Internal.Extensions;

internal static class GameManagerExtensions
{
    public static GameSession GetSessionOrThrow(this GameManager manager, Ulid sessionId)
    {
        (bool isSuccess, GameSession session, _) = manager.GetSession(sessionId);

        if (!isSuccess)
        {
            throw new InvalidOperationException($"Session with ID {sessionId} not found.");
        }

        return session;
    }

    public static Board GetBoardOrThrow(this GameManager manager, Ulid sessionId)
    {
        GameSession session = manager.GetSessionOrThrow(sessionId);
        return manager.GetBoard(session);
    }
}
