// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefenseGame.Api.Game;
using DefenseGame.Contracts.Views;
using Microsoft.Extensions.Logging;
using R3;
using VContainer.Unity;
using ZLogger;

namespace App.Scenes.DefenseGame
{
    public class DefenseGameEntryPoint : IInitializable, IAsyncStartable, IDisposable
    {
        private readonly GameController _controller;
        private readonly DefenseGamePresenter _presenter;
        private readonly ILogger<DefenseGameEntryPoint> _logger;

        private DisposableBag _disposables = new();

        public DefenseGameEntryPoint(
            GameController controller,
            DefenseGamePresenter presenter,
            ILogger<DefenseGameEntryPoint> logger)
        {
            _controller = controller;
            _presenter = presenter;
            _logger = logger;
        }

        public void Initialize()
        {
        }

        public async UniTask StartAsync(CancellationToken ct = default)
        {
            var createGameResult = await _controller.CreateGame(ct);
            if (createGameResult.IsError)
            {
                _logger.ZLogError($"Failed to create game: {createGameResult.GetErrors().ToArray()}");
                return;
            }

            (Ulid sessionId, IGameStateView stateView) = createGameResult.Value;
            _presenter.SetRadius(stateView.DefenseZone.Radius);

            stateView.Phase.Subscribe(_logger, static (phase, state) =>
            {
                state.ZLogDebug($"Game phase changed: {phase}");
            }).AddTo(ref _disposables);

            stateView.OnSelectingCards.Subscribe(_logger, static (_, state) =>
            {
                state.ZLogDebug($"Selecting cards phase started");
            }).AddTo(ref _disposables);

            // 1. 게임 시작
            // 2. 카드 셀렉팅
            // 3. 전투 (적군 스폰)
            var gameResult = await _controller.RunGame(static async (ctx, ct) =>
            {
                // 콜드 스타트 시 게임 데이터 준비 처리.
                // 두 번째 부터는 게임 데이터가 준비되어 있어 바로 시작.
                var selecting = await ctx.Begin(ct);

                // 첫 시작 시 카드 셀렉팅 2번
                // 덱에 있는 유닛 중 스폰되지 않은 유닛소환 카드 1~2개 포함
                // 소환되어 있는 유닛의 스킬 획득 카드 셀렉팅
                var battle = await selecting.Execute(ct);
                // 전투 시작
                // 적군 스폰
                // 경험치 획득으로 레벨업 시 넘어감
                await battle.Execute(ct);
            }, ct);

            if (gameResult.IsError)
            {
                _logger.ZLogError($"Game execution failed: {gameResult.GetErrors().ToArray()}");
                return;
            }
            _logger.ZLogInformation($"Game execution completed successfully");
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
