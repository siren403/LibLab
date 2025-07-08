// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using DefenseGame.Core.Internal.Entities;
using GameKit.Common.Results;

namespace DefenseGame.Internal.Repositories
{
    internal interface IUnitDeckRepository
    {
        Result AddDeck(UnitDeck deck);
        Result<UnitDeck> GetDeck(Ulid deckId);
    }
}
