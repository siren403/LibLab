// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DefenseGame.Contracts.ValueObjects;
using DefenseGame.Core.ValueObjects;

namespace DefenseGame.Core.Internal.Entities
{
    internal record UnitSpec(EntityId Id, string Name, Hp Hp, Atk Atk);
}
