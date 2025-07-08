// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DefenseGame.Contracts.Enums;
using DefenseGame.Contracts.ValueObjects;
using DefenseGame.Contracts.Views;
using GameKit.GameSessions.Core;
using R3;
using Result = GameKit.Common.Results.Result;

namespace DefenseGame.Core.Internal.Entities
{
    internal class GameState : IGameState, IGameStateView
    {
        internal GameState()
        {

        }

        public DefenseZone DefenseZone { get; init; }

        public ReadOnlyReactiveProperty<GamePhase> Phase => _phase;
        public Observable<R3.Unit> OnSelectingCards => _onSelectingCards;

        private readonly ReactiveProperty<GamePhase> _phase = new(GamePhase.None);
        private readonly Subject<R3.Unit> _onSelectingCards = new();

        public Result NextPhase()
        {
            switch (_phase.Value)
            {
                case GamePhase.None:
                    _phase.Value = GamePhase.Ready;
                    return Result.Ok;
                case GamePhase.Ready:
                    _phase.Value = GamePhase.SelectingCards;
                    _onSelectingCards.OnNext(R3.Unit.Default);
                    return Result.Ok;
                case GamePhase.SelectingCards:
                    _phase.Value = GamePhase.Battle;
                    return Result.Ok;
                case GamePhase.Battle:
                    // Handle battle phase logic here
                    return Result.Ok;
            }

            return Result.Fail(
                $"{nameof(GameState)}.FailedNextPhase",
                $"Cannot transition from phase {_phase.Value} to the next phase."
            );
        }
    }
}
