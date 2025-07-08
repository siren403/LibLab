using System;
using System.Collections.Generic;
using DefenseGame.Common.Results;
using DefenseGame.Contracts.ValueObjects;

namespace DefenseGame.Core.Features.GameSessions
{
    internal partial class GameSessionManager
    {
        private readonly Dictionary<Ulid, GameSession> _activeSessions = new();

        public GameSession CreateGameSession(float radius)
        {
            var sessionId = CreateSessionId();

            var zone = DefenseZone.FromRadius(radius);
            var session = GameSession.Create(sessionId, zone);
            _activeSessions[sessionId] = session;

            return session;
        }

        public Result<GameSession> GetSession(Ulid sessionId)
        {
            if (_activeSessions.TryGetValue(sessionId, out var session))
            {
                return Result<GameSession>.Ok(session);
            }

            return Result<GameSession>.Fail($"Session with ID {sessionId} not found.");
        }
    }

    /// <summary>
    /// Generate ids
    /// </summary>
    internal partial class GameSessionManager
    {
        private Ulid CreateSessionId(int retry = 3)
        {
            if (retry <= 0)
            {
                throw new InvalidOperationException("Failed to create a unique session ID after multiple attempts.");
            }

            var sessionId = Ulid.NewUlid();
            return _activeSessions.ContainsKey(sessionId) ? CreateSessionId(retry - 1) : sessionId;
        }
    }
}
