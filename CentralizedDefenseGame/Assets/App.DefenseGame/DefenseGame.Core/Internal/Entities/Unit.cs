// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using DefenseGame.Contracts.ValueObjects;
using DefenseGame.Contracts.Views;
using R3;

namespace DefenseGame.Core.Internal.Entities
{
    internal partial class Unit : IUnitView
    {
        public Ulid Id { get; private init; }

        private readonly ReactiveProperty<Hp> _currentHp;
        private readonly ReactiveProperty<Hp> _maxHp;
        private readonly ReactiveProperty<Atk> _currentAtk;

        public ReadOnlyReactiveProperty<Hp> CurrentHp => _currentHp;
        public ReadOnlyReactiveProperty<Hp> MaxHp => _maxHp;
        public ReadOnlyReactiveProperty<Atk> Atk => _currentAtk;

        private Unit(Hp hp, Atk atk)
        {
            _currentHp = new ReactiveProperty<Hp>(hp);
            _maxHp = new ReactiveProperty<Hp>(hp);
            _currentAtk = new ReactiveProperty<Atk>(atk);
        }
    }

    internal partial class Unit
    {
        public static Unit FromSpec(Ulid id, UnitSpec spec)
        {
            return new Unit(spec.Hp, spec.Atk) { Id = id };
        }
    }
}
