// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using DefenseGame.Core.Internal.Entities;
using DefenseGame.Internal.Repositories;

namespace DefenseGame.Infrastructure.Internal.Repositories.InMemory
{
    internal class InMemoryUnitRepository : IUnitRepository
    {
        private readonly Units _units = new();

        public void AddUnit(Ulid sessionId, Unit unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit), "Unit cannot be null.");
            }

            if (unit.Id == Ulid.Empty)
            {
                throw new ArgumentException("Unit ID cannot be empty.", nameof(unit));
            }

            _units.AddUnit(sessionId, unit);
        }

        private class Units : Dictionary<Ulid, List<Unit>>
        {
            public void AddUnit(Ulid sessionId, Unit unit)
            {
                if (!TryGetValue(sessionId, out var units))
                {
                    units = new List<Unit>();
                    this[sessionId] = units;
                }

                units.Add(unit);
            }
        }
    }
}
