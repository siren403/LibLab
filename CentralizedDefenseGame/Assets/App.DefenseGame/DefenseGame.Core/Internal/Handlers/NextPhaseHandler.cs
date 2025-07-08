// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using DefenseGame.Contracts.Enums;
using DefenseGame.Core.Application.Commands;
using DefenseGame.Core.Internal.Entities;
using GameKit.Common.Results;
using GameKit.GameSessions.Core.Commands;
using GameKit.GameSessions.VContainer;
using VExtensions.Mediator.Abstractions;

namespace DefenseGame.Core.Internal.Handlers
{
    internal class NextPhaseHandler : ICommandHandler<NextPhaseCommand, Result>
    {
        private readonly IMediator _mediator;

        public NextPhaseHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async UniTask<Result> ExecuteAsync(NextPhaseCommand command, CancellationToken ct)
        {
            var result = await _mediator.ExecuteGetGameState(new GetGameStateCommand<GameState>()
            {
                SessionId = command.SessionId
            }, ct);

            if (result.IsError(out Result<GameState> fail))
            {
                return fail;
            }

            GameState state = result.Value;
            return state.NextPhase();
        }
    }
}
