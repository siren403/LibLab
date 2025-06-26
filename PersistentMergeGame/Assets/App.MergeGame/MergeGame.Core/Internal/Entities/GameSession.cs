// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using App.MergeGame.Core.Internal.Enums;

namespace MergeGame.Core.Internal.Entities
{
    internal class GameSession
    {
        public Ulid Id { get; private init; }
        public Ulid BoardId { get; private init; }
        public DateTime CreatedAt { get; private init; }
        public DateTime? EndedAt { get; set; }
        public GameState State { get; set; }

        public static GameSession CreateWaitingSession(Ulid sessionId, Ulid boardId)
        {
            return new GameSession()
            {
                Id = sessionId,
                BoardId = boardId,
                CreatedAt = DateTime.UtcNow,
                EndedAt = null,
                State = GameState.WaitingToStart
            };
        }

        public override string ToString()
        {
            return $"{nameof(GameSession)}(Id: {Id}, {nameof(BoardId)}: {BoardId}, {nameof(CreatedAt)}: {CreatedAt}, {nameof(EndedAt)}: {EndedAt}, {nameof(State)}: {State})";
        }
    }
}
