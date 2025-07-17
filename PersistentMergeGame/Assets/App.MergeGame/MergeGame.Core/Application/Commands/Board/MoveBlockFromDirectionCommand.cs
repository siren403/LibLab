using System;
using GameKit.Common.Results;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Application.Commands.Board
{
    public struct MoveBlockFromDirectionCommand : ICommand<FastResult<Position>>
    {
        public Ulid SessionId { get; init; }
        public Position FromPosition { get; init; }
        public Direction Direction { get; init; }
    }
}
