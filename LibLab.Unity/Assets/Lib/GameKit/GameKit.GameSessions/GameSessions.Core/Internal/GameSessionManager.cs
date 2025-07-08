using System;
using System.Collections.Generic;
using GameKit.Common.Results;

namespace GameKit.GameSessions.Core.Internal
{
    internal partial class GameSessionManager<TGameState> where TGameState : IGameState
    {
        private readonly Dictionary<Ulid, GameSession<TGameState>> _activeSessions = new();

        public GameSession<TGameState> CreateGameSession(TGameState state)
        {
            var sessionId = CreateSessionId();
            var session = GameSession<TGameState>.Create(sessionId, state);
            _activeSessions[sessionId] = session;

            return session;
        }

        public Result<GameSession<TGameState>> GetSession(Ulid sessionId)
        {
            return _activeSessions.TryGetValue(sessionId, out var session)
                ? Result<GameSession<TGameState>>.Ok(session)
                : Result<GameSession<TGameState>>.Fail($"Session with ID {sessionId} not found.");

        }
    }

    /// <summary>
    /// Generate ids
    /// </summary>
    internal partial class GameSessionManager<TGameState>
    {
        private Ulid CreateSessionId(int retry = 3)
        {
            if (retry <= 0)
            {
                throw new InvalidOperationException(
                    "Failed to create a unique session ID after multiple attempts.");
            }

            var sessionId = Ulid.NewUlid();
            return _activeSessions.ContainsKey(sessionId) ? CreateSessionId(retry - 1) : sessionId;
        }
    }
}
