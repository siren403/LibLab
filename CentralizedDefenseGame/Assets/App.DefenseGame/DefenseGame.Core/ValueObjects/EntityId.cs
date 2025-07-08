// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnitGenerator;

namespace DefenseGame.Core.ValueObjects
{
    [UnitOf(typeof(long), UnitGenerateOptions.ImplicitOperator)]
    public readonly partial struct EntityId
    {
    }
}
