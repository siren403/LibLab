// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using DefenseGame.Core.Internal.Entities;
using DefenseGame.Internal.Repositories;
using GameKit.Common.Results;

namespace DefenseGame.Infrastructure.Internal.Repositories.InMemory
{
    internal class InMemoryUnitDeckRepository : IUnitDeckRepository
    {
        private readonly Dictionary<Ulid, UnitDeck> _decks = new();

        public Result AddDeck(UnitDeck deck)
        {
            return _decks.TryAdd(deck.Id, deck)
                ? Result.Ok
                : Result.Fail($"{nameof(UnitDeck)}.AlreadyExists", $"Deck with ID {deck.Id} already exists.");
        }

        public Result<UnitDeck> GetDeck(Ulid deckId)
        {
            return Result<UnitDeck>.Ok(UnitDeck.Create(
                deckId,
                "soldier",
                "archer",
                "knight"
            ));
            return _decks.TryGetValue(deckId, out var deck)
                ? Result<UnitDeck>.Ok(deck)
                : Result<UnitDeck>.Fail($"{nameof(UnitDeck)}.NotFound", $"Deck with ID {deckId} not found.");
        }
    }
}
