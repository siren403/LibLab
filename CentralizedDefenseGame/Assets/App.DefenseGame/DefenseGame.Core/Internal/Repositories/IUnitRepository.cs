// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using DefenseGame.Core.Internal.Entities;
using GameKit.Common.Results;
using Void = GameKit.Common.Results.Void;

namespace DefenseGame.Internal.Repositories
{
    internal interface IUnitRepository
    {
        FastResult<Void> AddUnit(Ulid sessionId, Unit unit);
    }
}
