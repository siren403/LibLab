// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using DefenseGame.Contracts.Views;
using DefenseGame.Core.Internal.Entities;

namespace DefenseGame.Core.Application.Commands
{
    public struct CreateGameSessionData
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
