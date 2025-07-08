// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DefenseGame.Contracts.Views;
using DefenseGame.Core.Application.Commands;
using DefenseGame.Core.Extensions;
using GameKit.Common.Results;
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
                new CreateGameSessionCommand()
                {
                    Radius = radius
                },
                ct);

            if (result.IsError(out Result<CreateGameResponse> fail))
            {
                return fail;
            }

            (Ulid sessionId, IGameStateView stateView) = result.Value;

            return Result<CreateGameResponse>.Ok(new CreateGameResponse(sessionId, stateView));
        }

        public UniTask<Result> NextPhase(Ulid sessionId, CancellationToken ct = default)
        {
            return _mediator.ExecuteNextPhase(new NextPhaseCommand()
            {
                SessionId = sessionId
            }, ct);
        }

        public async UniTask<Result> RunGame(
            Func<GameContext, CancellationToken, UniTask> runner,
            CancellationToken ct = default
        )
        {

            GameContext context = new GameContext();
            do
            {
                await runner(context, ct);
                if (!context.IsStarted)
                {
                    return Result.Fail(
                        $"{nameof(RunGame)}.NotStarted",
                        "Game has not started yet."
                    );
                }
            } while (context.IsPlaying && !ct.IsCancellationRequested);

            if (ct.IsCancellationRequested)
            {
                return Result.Fail(
                    $"{nameof(RunGame)}.Cancelled",
                    "Game was cancelled by user."
                );
            }

            return Result.Ok;
        }
    }

    public class GameContext
    {
        public bool IsStarted { get; private set; } = false;
        public bool IsPlaying { get; internal set; } = false;

        public UniTask<CardSelectingExecutor> Begin(CancellationToken ct)
        {
            IsStarted = true;
            IsPlaying = true;
            return UniTask.FromResult(new CardSelectingExecutor(this));
        }
    }

    public class CardSelectingExecutor
    {
        private readonly GameContext _context;

        public CardSelectingExecutor(GameContext context)
        {
            _context = context;

        }

        public UniTask<BattleExecutor> Execute(CancellationToken ct)
        {
            return UniTask.FromResult(new BattleExecutor(_context));
        }
    }

    public class BattleExecutor
    {
        private readonly GameContext _context;

        public BattleExecutor(GameContext context)
        {
            _context = context;
        }

        public UniTask Execute(CancellationToken ct)
        {
            // 전투 시작
            // 적군 스폰 데이터 퍼블리시
            // 경험치 획득으로 레벨업 시 넘어감
            // 유닛 전부 사망 시 게임 종료
            _context.IsPlaying = false;
            return UniTask.CompletedTask;
        }
    }

}
