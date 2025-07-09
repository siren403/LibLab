// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DefenseGame.Core.Internal.Entities;
using DefenseGame.Core.ValueObjects;
using GameKit.Common.Results;

namespace DefenseGame.Internal.Repositories
{
    internal interface IUnitSpecRepository
    {
        FastResult<UnitSpec> GetById(EntityId id);
        FastResult<UnitSpec> GetByName(string name);
    }
}
