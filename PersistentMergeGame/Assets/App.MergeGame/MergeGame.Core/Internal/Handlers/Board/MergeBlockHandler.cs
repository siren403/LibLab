// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Data;
using MergeGame.Core.Internal.Managers;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board
{
    internal class MergeBlockHandler : ICommandHandler<MergeBlockCommand, MergeBlockResult>
    {
        private readonly GameManager _manager;

        public MergeBlockHandler(GameManager manager)
        {
            _manager = manager;
        }

        public UniTask<MergeBlockResult> ExecuteAsync(MergeBlockCommand command, CancellationToken ct)
        {
            (bool isSuccess, Entities.GameSession session, _) = _manager.GetSession(command.SessionId);
            if (!isSuccess)
            {
                throw new InvalidOperationException($"Session with ID {command.SessionId} not found.");
            }

            var board = _manager.GetBoard(session);
            var mergeResult = board.MergeBlock(command.FromPosition, command.ToPosition);

            if (mergeResult is not Ok<ValueObjects.MergeResult> (_, var (fromCell, toCell), _))
            {
                throw new InvalidOperationException(
                    $"Failed to merge blocks at positions {command.FromPosition} and {command.ToPosition}.");
            }

            return UniTask.FromResult(new MergeBlockResult(
                BoardCell.FromEntity(fromCell),
                BoardCell.FromEntity(fromCell) with { X = toCell.Position.X, Y = toCell.Position.Y },
                BoardCell.FromEntity(toCell)
            ));
        }
    }
}
