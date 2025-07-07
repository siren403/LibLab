// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using MergeGame.Common.Results;
using MergeGame.Core.Internal.Entities;
using MergeGame.Core.Internal.Managers;

namespace MergeGame.Core.Internal.Extensions;

internal static class GameManagerExtensions
{
    public static GameSession GetSessionOrThrow(this GameManager manager, Ulid sessionId)
    {
        var result = manager.GetSession(sessionId);

        if (result.IsError)
        {
            throw new InvalidOperationException($"Session with ID {sessionId} not found.");
        }

        return result.Value;
    }

    public static Board GetBoardOrThrow(this GameManager manager, Ulid sessionId)
    {
        GameSession session = manager.GetSessionOrThrow(sessionId);
        return manager.GetBoard(session);
    }

    public static Result<Board> GetBoardOrError(this GameManager manager, Ulid sessionId)
    {
        var result = manager.GetSession(sessionId);
        if (result.IsError(out Result<Board> sessionFail))
        {
            return sessionFail;
        }

        GameSession session = result.Value;
        return Result<Board>.Ok(manager.GetBoard(session));
    }
}
