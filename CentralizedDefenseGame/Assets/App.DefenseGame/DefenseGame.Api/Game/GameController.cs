// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using DefenseGame.Common.Results;
using DefenseGame.Core.Features.GameSessions;
using VExtensions.Mediator.Abstractions;

namespace DefenseGame.Api.Game
{
    public class GameController
    {
        private readonly IMediator _mediator;

        public GameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async UniTask<Result<CreateGameResponse>> CreateGame(CancellationToken ct = default)
        {
            float radius = 8;
            var result = await _mediator.ExecuteCreateGameSession(
                new CreateGameSessionCommand() { Radius = radius },
                ct);

            if (result.IsError(out Result<CreateGameResponse> fail))
            {
                return fail;
            }

            var sessionId = result.Value;
            return Result<CreateGameResponse>.Ok(new CreateGameResponse(sessionId, radius));
        }
    }
}
