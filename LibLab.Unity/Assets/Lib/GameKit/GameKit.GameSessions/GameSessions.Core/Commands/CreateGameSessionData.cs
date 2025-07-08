// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;

namespace GameKit.GameSessions.Core.Commands
{
    public record CreateGameSessionData<TGameState>(Ulid SessionId, TGameState State) where TGameState : IGameState;
}
