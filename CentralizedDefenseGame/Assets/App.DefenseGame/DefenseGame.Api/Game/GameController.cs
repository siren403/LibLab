// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefenseGame.Contracts.Views;
using DefenseGame.Core.Application.Commands;
using DefenseGame.Core.Extensions;
using GameKit.Common.Results;
using Microsoft.Extensions.Logging;
using VExtensions.Mediator.Abstractions;
using VitalRouter;
using VitalRouter.R3;
using Unit = R3.Unit;
using Void = GameKit.Common.Results.Void;

namespace DefenseGame.Api.Game
{
    public class GameController
    {
        private readonly IMediator _mediator;
        private readonly Router _router;
        private readonly ILoggerFactory _loggerFactory;

        public GameController(IMediator mediator, Router router, ILoggerFactory loggerFactory)
        {
            _mediator = mediator;
            _router = router;
            _loggerFactory = loggerFactory;
        }

        public async UniTask<FastResult<CreateGameResponse>> CreateGameAsync(CancellationToken ct = default)
        {
            float radius = 8;
            var result = await _mediator.ExecuteCreateGameSession(
                new CreateGameSessionCommand() { Radius = radius },
                ct);

            if (result.IsError(out FastResult<CreateGameResponse> fail))
            {
                return fail;
            }

            (Ulid sessionId, IGameStateView stateView) = result.Value;

            return FastResult<CreateGameResponse>.Ok(new CreateGameResponse
            {
                SessionId = sessionId, StateView = stateView
            });
        }

        public async UniTask<FastResult<Void>> RunGameAsync(
            Ulid sessionId,
            ChapterStage stage,
            Func<GameContext, CancellationToken, UniTask> runner,
            CancellationToken ct = default
        )
        {
            GameContext context = new(sessionId, _router, _loggerFactory, ct);
            try
            {
                await runner(context, context.CancellationToken);
                if (!context.IsStarted)
                {
                    return FastResult.Fail(
                        $"{nameof(RunGameAsync)}.NotStarted",
                        "Game has not started yet."
                    );
                }
            }
            catch (OperationCanceledException canceled)
            {
                return context.GameResult;
            }

            return context.GameResult;
        }
    }
}
