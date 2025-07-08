// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using DefenseGame.Core.Internal.Entities;

namespace DefenseGame.Internal.Repositories
{
    internal interface IUnitRepository
    {
        void AddUnit(Ulid sessionId, Unit unit);
    }
}
