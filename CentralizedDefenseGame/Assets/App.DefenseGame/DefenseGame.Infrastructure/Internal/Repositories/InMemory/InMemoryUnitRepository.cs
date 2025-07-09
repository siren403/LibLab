// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using DefenseGame.Core.Internal.Entities;
using DefenseGame.Internal.Repositories;
using GameKit.Common.Results;
using Void = GameKit.Common.Results.Void;

namespace DefenseGame.Infrastructure.Internal.Repositories.InMemory
{
    internal class InMemoryUnitRepository : IUnitRepository
    {
        private readonly Units _units = new();

        public FastResult<Void> AddUnit(Ulid sessionId, Unit unit)
        {
            if (unit == null!)
            {
                return FastResult.Fail(
                    $"{nameof(Unit)}.NullReference",
                    "Unit cannot be null."
                );
            }

            if (unit.Id == Ulid.Empty)
            {
                return FastResult.Fail(
                    $"{nameof(Unit)}.InvalidId",
                    "Unit ID cannot be empty."
                );
            }

            _units.AddUnit(sessionId, unit);
            return FastResult.Ok;
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
