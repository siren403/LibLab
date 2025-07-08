// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace DefenseGame.Core.Internal.Entities
{
    public class UnitDeck
    {
        private UnitDeck(Ulid id, List<string> units)
        {
            Id = id;
            _units = units;
        }

        public Ulid Id { get; }
        public IReadOnlyList<string> Units => _units.AsReadOnly();

        private readonly List<string> _units;

        public static UnitDeck Create(Ulid id, params string[] units)
        {
            if (id == default)
            {
                throw new ArgumentException("UnitDeck ID cannot be default.", nameof(id));
            }

            return new UnitDeck(id, new List<string>(units));
        }
    }
}
