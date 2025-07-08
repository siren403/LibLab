// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DefenseGame.Contracts.ValueObjects;
using R3;

namespace DefenseGame.Contracts.Views
{
    public interface IUnitView
    {
        ReadOnlyReactiveProperty<Hp> CurrentHp { get; }
        ReadOnlyReactiveProperty<Hp> MaxHp { get; }
        ReadOnlyReactiveProperty<Atk> Atk { get; }
    }
}
