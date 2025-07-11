// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DefenseGame.Api.Game;
using DefenseGame.Contracts.Views;
using GameKit.Common.Results;
using Microsoft.Extensions.Logging;
using R3;
using VContainer.Unity;
using VitalRouter;
using ZLogger;
using Void = GameKit.Common.Results.Void;

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
            var createGameResult = await _controller.CreateGameAsync(ct);
            if (createGameResult.IsError)
            {
                _logger.ZLogError($"Failed to create game: {createGameResult.GetErrors().ToArray()}");
                return;
            }

            (Ulid sessionId, IGameStateView stateView) = createGameResult.Value;
            _presenter.SetRadius(stateView.DefenseZone.Radius);

            stateView.Phase
                .Subscribe(_logger, static (phase, state) => { state.ZLogDebug($"Game phase changed: {phase}"); })
                .AddTo(ref _disposables);

            stateView.OnSelectingCards
                .Subscribe(_logger, static (_, state) => { state.ZLogDebug($"Selecting cards phase started"); })
                .AddTo(ref _disposables);


            /*
             * 1. ChapterStage
             *  if first
             *   - leveled up * 2
             *   - show skills (3)
             *   choose skills (1 ~ 2)
             *   reroll
             * 2. Battle
             *   - spawn enemies
             *   - gain experience
             *   if leveled up
             *     - show skills (3)
             *     choose skills (1 ~ 2)
             *     reroll
             *   if wave cleared
             *     has skill rewards
             *       - play skill rewards animation
             * 3. Repeat
             */

            var gameResult = await _controller.RunGameAsync(sessionId, new(
            ) { }, static async (ctx, ct) =>
            {
                BeginPhase begin = await ctx.BeginAsync(ct);
                begin.OnLeveledUp(OnBeginLeveledUpAsync).AddTo(ct);

                BattlePhase battle = await begin.RunAsync(ct);
                battle.OnLeveledUp(OnBattleLeveledUpAsync).AddTo(ct);

                await battle.RunAsync(ct);
            }, ct);

            if (gameResult.IsError)
            {
                _logger.ZLogError($"Game execution failed: {gameResult.GetErrors().ToArray()}");
                return;
            }

            _logger.ZLogInformation($"Game execution completed successfully");

            return;

            static async ValueTask OnBeginLeveledUpAsync(LeveledUpCommand<BeginPhase> command, PublishContext ctx)
            {
                BeginPhase begin = command.Phase;
                begin.Logger.ZLogDebug(
                    $"Leveled up in begin phase: Skill1={command.Skill1}, Skill2={command.Skill2}, Skill3={command.Skill3}");
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ctx.CancellationToken);
                var selectedSkillId = await begin.SelectSkillAsync(command.Skill1, ctx.CancellationToken);

                begin.Logger.ZLogDebug(
                    $"Skills selected in begin phase: Skill1={command.Skill1}, Skill2={command.Skill2}, Skill3={command.Skill3}");
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ctx.CancellationToken);
            }

            static async ValueTask OnBattleLeveledUpAsync(LeveledUpCommand<BattlePhase> command, PublishContext ctx)
            {
                BattlePhase battle = command.Phase;
                battle.Logger.ZLogDebug(
                    $"Leveled up in battle: Skill1={command.Skill1}, Skill2={command.Skill2}, Skill3={command.Skill3}");
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ctx.CancellationToken);
                var selectedSkillId = await battle.SelectSkillAsync(command.Skill1, ctx.CancellationToken);

                battle.Logger.ZLogDebug(
                    $"Skills selected in battle: Skill1={command.Skill1}, Skill2={command.Skill2}, Skill3={command.Skill3}");
            }
        }


        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
