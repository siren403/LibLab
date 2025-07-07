// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using MergeGame.Common.Results;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Application.Commands.GameSession
{
    public struct CreateGameSessionCommand : ICommand<Result<Ulid>>
    {
        public int Width { get; init; }
        public int Height { get; init; }
    }
}
