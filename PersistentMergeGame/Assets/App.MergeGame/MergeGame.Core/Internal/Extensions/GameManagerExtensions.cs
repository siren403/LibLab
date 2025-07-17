// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using GameKit.Common.Results;
using MergeGame.Core.Internal.Entities;
using MergeGame.Core.Internal.Managers;

namespace MergeGame.Core.Internal.Extensions;

internal static class GameManagerExtensions
{
    [Obsolete]
    public static GameSession GetSessionOrThrow(this GameManager manager, Ulid sessionId)
    {
        var result = manager.GetSession(sessionId);

        if (result.IsError)
        {
            throw new InvalidOperationException($"Session with ID {sessionId} not found.");
        }

        return result.Value;
    }

    [Obsolete]
    public static Board GetBoardOrThrow(this GameManager manager, Ulid sessionId)
    {
        GameSession session = manager.GetSessionOrThrow(sessionId);
        return manager.GetBoard(session);
    }

    public static FastResult<Board> GetBoardOrError(this GameManager manager, Ulid sessionId)
    {
        var result = manager.GetSession(sessionId);
        if (result.IsError(out FastResult<Board> sessionFail))
        {
            return sessionFail;
        }

        GameSession session = result.Value;
        return FastResult<Board>.Ok(manager.GetBoard(session));
    }
}
