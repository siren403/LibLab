// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using DefenseGame.Common.Results;
using VExtensions.Mediator.Abstractions;

namespace DefenseGame.Core.Features.GameSessions
{
    public struct CreateGameSessionCommand : ICommand<Result<Ulid>>
    {
        public float Radius { get; init; }
    }
}
