// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DefenseGame.Contracts.Enums;
using DefenseGame.Core.ValueObjects;

namespace DefenseGame.Core.Internal.Entities
{
    internal record Card(EntityId Id, string Name, CardType Type);
}
