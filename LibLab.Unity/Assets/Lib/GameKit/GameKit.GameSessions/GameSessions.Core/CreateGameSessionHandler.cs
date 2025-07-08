// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using VExtensions.Mediator.Abstractions;

namespace GameKit.GameSessions.Core
{
    internal class CreateGameSessionHandler<TGameState>
        : ICommandHandler<CreateGameSessionCommand<TGameState>, Result<Ulid>>
        where TGameState : IGameState
    {
        private readonly GameSessionManager<TGameState> _sessionManager;

        public CreateGameSessionHandler(GameSessionManager<TGameState> sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public UniTask<Result<Ulid>> ExecuteAsync(CreateGameSessionCommand<TGameState> command,
            CancellationToken ct)
        {
            var session = _sessionManager.CreateGameSession(command.State);
            var result = Result<Ulid>.Ok(session.Id);
            return result;
        }
    }
}
