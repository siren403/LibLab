// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using GameKit.Common.Results;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Application.Commands.GameSession
{
    public struct CreateStartingGameSessionCommand : ICommand<FastResult<Ulid>>
    {
    }
}
