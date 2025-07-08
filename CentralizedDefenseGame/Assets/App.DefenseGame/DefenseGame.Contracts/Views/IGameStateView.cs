// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DefenseGame.Contracts.Enums;
using DefenseGame.Contracts.ValueObjects;
using R3;

namespace DefenseGame.Contracts.Views
{
    public interface IGameStateView
    {
        DefenseZone DefenseZone { get; }
        ReadOnlyReactiveProperty<GamePhase> Phase { get; }

        Observable<Unit> OnSelectingCards { get; }
    }
}
