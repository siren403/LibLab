// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using GameKit.GameSessions.Core.Commands;
using VExtensions.Mediator.Abstractions;

namespace GameKit.GameSessions.Core.Internal.Handlers
{
    internal class CreateGameSessionHandler<TGameState>
        : ICommandHandler<CreateGameSessionCommand<TGameState>, Result<CreateGameSessionData<TGameState>>>
        where TGameState : IGameState
    {
        private readonly GameSessionManager<TGameState> _sessionManager;

        public CreateGameSessionHandler(GameSessionManager<TGameState> sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public UniTask<Result<CreateGameSessionData<TGameState>>> ExecuteAsync(
            CreateGameSessionCommand<TGameState> command,
            CancellationToken ct)
        {
            var session = _sessionManager.CreateGameSession(command.State);
            return Result<CreateGameSessionData<TGameState>>.Ok(new CreateGameSessionData<TGameState>(
                session.Id,
                session.State
            ));
        }
    }
}
