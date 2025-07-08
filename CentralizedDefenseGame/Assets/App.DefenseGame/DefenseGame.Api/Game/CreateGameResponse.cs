// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using DefenseGame.Contracts.Views;

namespace DefenseGame.Api.Game
{
    public record CreateGameResponse(Ulid SessionId, IGameStateView StateView);
}
