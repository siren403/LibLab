// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using DefenseGame.Contracts.ValueObjects;

namespace DefenseGame.Core.Features.GameSessions
{
    internal class GameSession
    {
        public Ulid Id { get; private init; }
        public DateTime CreatedAt { get; private init; }
        public DateTime? EndedAt { get; private set; }

        public DefenseZone DefenseZone { get; private init; }

        public static GameSession Create(Ulid sessionId, DefenseZone defenseZone)
        {
            return new GameSession()
            {
                Id = sessionId, CreatedAt = DateTime.UtcNow, EndedAt = null, DefenseZone = defenseZone
            };
        }

        public override string ToString()
        {
            return
                $"{nameof(GameSession)}(Id: {Id}, {nameof(CreatedAt)}: {CreatedAt}, {nameof(EndedAt)}: {EndedAt})";
        }
    }
}
