// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using DefenseGame.Core.Internal.Entities;
using DefenseGame.Core.ValueObjects;
using DefenseGame.Internal.Repositories;
using GameKit.Common.Results;

namespace DefenseGame.Infrastructure.Internal.Repositories.InMemory
{
    internal class InMemoryUnitSpecRepository : IUnitSpecRepository
    {
        private readonly Dictionary<EntityId, UnitSpec> _entitiesById = new();
        private readonly Dictionary<string, UnitSpec> _entitiesByName = new();

        public InMemoryUnitSpecRepository()
        {
            AddSpec(new UnitSpec(1, "soldier", 100, 10));
            AddSpec(new UnitSpec(2, "archer", 80, 15));
            AddSpec(new UnitSpec(3, "knight", 150, 20));
            AddSpec(new UnitSpec(4, "mage", 60, 25));
            AddSpec(new UnitSpec(5, "healer", 70, 5));

            return;

            void AddSpec(UnitSpec spec)
            {
                _entitiesById.Add(spec.Id, spec);
                _entitiesByName.Add(spec.Name, spec);
            }
        }

        public FastResult<UnitSpec> GetById(EntityId id)
        {
            if (_entitiesById.TryGetValue(id, out var spec))
            {
                return FastResult<UnitSpec>.Ok(spec);
            }

            return FastResult<UnitSpec>.Fail(
                $"{nameof(UnitSpec)}.NotFound",
                $"UnitSpec with ID {id} not found."
            );
        }

        public FastResult<UnitSpec> GetByName(string name)
        {
            if (_entitiesByName.TryGetValue(name, out var spec))
            {
                return FastResult<UnitSpec>.Ok(spec);
            }

            return FastResult<UnitSpec>.Fail(
                $"{nameof(UnitSpec)}.NotFound",
                $"UnitSpec with name '{name}' not found."
            );
        }
    }
}
