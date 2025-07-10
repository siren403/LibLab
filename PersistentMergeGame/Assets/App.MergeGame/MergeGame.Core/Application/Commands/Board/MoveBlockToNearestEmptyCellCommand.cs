using System;
using GameKit.Common.Results;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Application.Commands.Board
{
    public readonly struct MoveBlockToNearestEmptyCellCommand : ICommand<FastResult<Position>>
    {
        public Ulid SessionId { get; init; }
        public Position FromPosition { get; init; }
        public Position ToPosition { get; init; }
    }
}
