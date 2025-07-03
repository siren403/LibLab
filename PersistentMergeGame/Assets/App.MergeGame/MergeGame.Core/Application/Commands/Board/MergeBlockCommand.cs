// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using MergeGame.Core.Application.Data;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Application.Commands.Board
{
    public struct MergeBlockCommand : ICommand<MergeBlockResult>
    {
        public Ulid SessionId { get; init; }
        public Position FromPosition { get; init; }
        public Position ToPosition { get; init; }
    }

    public record MergeBlockResult(
        BoardCell FromCell,
        BoardCell ToCell,
        BoardCell SpawnedCell
    );
}
