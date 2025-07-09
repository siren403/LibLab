// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using GameKit.Common.Results;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;
using Void = GameKit.Common.Results.Void;

namespace MergeGame.Core.Application.Commands.Board;

public readonly struct MoveBlockCommand : ICommand<FastResult<Void>>
{
    public Ulid SessionId { get; init; }
    public Position FromPosition { get; init; }
    public Position ToPosition { get; init; }
}
