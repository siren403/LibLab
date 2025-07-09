// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using GameKit.Common.Results;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Application.Commands.Board;

public struct CheckEmptyCellCommand : ICommand<FastResult<bool>>
{
    public Ulid SessionId { get; init; }
    public Position Position { get; init; }
}
