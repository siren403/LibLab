// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefenseGame.Contracts.ValueObjects;
using DefenseGame.Core.Application.Commands;
using DefenseGame.Core.Internal.Entities;
using GameKit.Common.Results;
using GameKit.GameSessions.Core.Commands;
using GameKit.GameSessions.VContainer;
using VExtensions.Mediator.Abstractions;

namespace DefenseGame.Core.Internal.Handlers
{
    internal class
        CreateGameSessionHandler : ICommandHandler<CreateGameSessionCommand, Result<CreateGameSessionData>>
    {
        private readonly IMediator _mediator;

        public CreateGameSessionHandler(IMediator mediator)
        {
            _mediator = mediator;

        }

        public async UniTask<Result<CreateGameSessionData>> ExecuteAsync(CreateGameSessionCommand command,
            CancellationToken ct)
        {
            var result = await _mediator.ExecuteCreateGameSession(new CreateGameSessionCommand<GameState>()
            {
                State = new GameState()
                {
                    DefenseZone = DefenseZone.FromRadius(command.Radius)
                }
            }, ct);

            if (result.IsError(out Result<CreateGameSessionData> fail))
            {
                return fail;
            }

            (Ulid sessionId, GameState state) = result.Value;
            return Result<CreateGameSessionData>.Ok(
                new CreateGameSessionData(sessionId, state)
            );
        }
    }
}
