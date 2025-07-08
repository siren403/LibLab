// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DefenseGame.Core.Internal.Entities;
using DefenseGame.Core.ValueObjects;

namespace DefenseGame.Internal.Repositories
{
    internal interface IUnitSpecRepository
    {
        UnitSpec GetById(EntityId id);
        UnitSpec GetByName(string name);
    }
}
