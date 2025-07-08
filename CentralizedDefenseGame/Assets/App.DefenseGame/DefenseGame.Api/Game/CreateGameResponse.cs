// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;

namespace DefenseGame.Api.Game
{
    public record CreateGameResponse(Ulid SessionId, float Radius);
}
