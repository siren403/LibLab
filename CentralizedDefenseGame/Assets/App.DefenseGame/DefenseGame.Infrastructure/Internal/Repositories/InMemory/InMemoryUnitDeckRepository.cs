// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using DefenseGame.Core.Internal.Entities;
using DefenseGame.Internal.Repositories;
using GameKit.Common.Results;
using Void = GameKit.Common.Results.Void;

namespace DefenseGame.Infrastructure.Internal.Repositories.InMemory
{
    internal class InMemoryUnitDeckRepository : IUnitDeckRepository
    {
        private readonly Dictionary<Ulid, UnitDeck> _decks = new();

        public FastResult<Void> AddDeck(UnitDeck deck)
        {
            return _decks.TryAdd(deck.Id, deck)
                ? FastResult.Ok
                : FastResult.Fail($"{nameof(UnitDeck)}.AlreadyExists", $"Deck with ID {deck.Id} already exists.");
        }

        public FastResult<UnitDeck> GetDeck(Ulid deckId)
        {
            return FastResult<UnitDeck>.Ok(UnitDeck.Create(
                deckId,
                "soldier",
                "archer",
                "knight"
            ));
            return _decks.TryGetValue(deckId, out var deck)
                ? FastResult<UnitDeck>.Ok(deck)
                : FastResult<UnitDeck>.Fail($"{nameof(UnitDeck)}.NotFound", $"Deck with ID {deckId} not found.");
        }
    }
}
