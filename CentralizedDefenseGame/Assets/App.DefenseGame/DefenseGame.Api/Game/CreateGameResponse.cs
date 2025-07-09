// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using DefenseGame.Contracts.Views;

namespace DefenseGame.Api.Game
{
    public struct CreateGameResponse
    {
        public Ulid SessionId { get; init; }
        public IGameStateView StateView { get; init; }

        public void Deconstruct(out Ulid sessionId, out IGameStateView stateView)
        {
            sessionId = SessionId;
            stateView = StateView;
        }
    }
}
