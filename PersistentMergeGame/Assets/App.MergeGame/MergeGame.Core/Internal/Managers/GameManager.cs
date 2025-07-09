using System;
using System.Collections.Generic;
using GameKit.Common.Results;
using MergeGame.Core.Internal.Entities;

namespace MergeGame.Core.Internal.Managers
{
    internal partial class GameManager
    {
        private readonly Dictionary<Ulid, GameSession> _activeSessions = new Dictionary<Ulid, GameSession>();
        private readonly Dictionary<Ulid, Board> _boards = new Dictionary<Ulid, Board>();

        public GameSession CreateGameSession(int width, int height)
        {
            var sessionId = CreateSessionId();
            var boardId = CreateBoardId();

            var board = Board.CreateWithCells(boardId, width, height);
            _boards[boardId] = board;

            var session = GameSession.CreateWaitingSession(sessionId, boardId);
            _activeSessions[sessionId] = session;

            return session;
        }

        public FastResult<GameSession> GetSession(Ulid sessionId)
        {
            return _activeSessions.TryGetValue(sessionId, out var session)
                ? FastResult<GameSession>.Ok(session)
                : FastResult<GameSession>.Fail($"Session with ID {sessionId} not found.");
        }

        public Board GetBoard(GameSession session)
        {
            return _boards[session.BoardId];
        }
    }

    /// <summary>
    /// Generate ids
    /// </summary>
    internal partial class GameManager
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

        private Ulid CreateBoardId(int retry = 3)
        {
            if (retry <= 0)
            {
                throw new InvalidOperationException("Failed to create a unique board ID after multiple attempts.");
            }

            var boardId = Ulid.NewUlid();
            return _boards.ContainsKey(boardId) ? CreateBoardId(retry - 1) : boardId;
        }
    }
}
