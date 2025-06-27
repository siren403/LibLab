// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Enums;
using MergeGame.Core.Internal.Managers;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board
{
    internal class IsMovableCellHandler : ICommandHandler<IsMovableCellCommand, bool>
    {
        private readonly GameManager _manager;

        public IsMovableCellHandler(GameManager manager)
        {
            _manager = manager;
        }

        public UniTask<bool> ExecuteAsync(IsMovableCellCommand command, CancellationToken ct)
        {
            (bool isSuccess, Entities.GameSession session, _) = _manager.GetSession(command.SessionId);
            if (!isSuccess)
            {
                throw new InvalidOperationException($"Session with ID {command.SessionId} not found.");
            }

            var board = _manager.GetBoard(session);
            var cell = board.GetCell(command.Position);

            bool result = cell is { HasBlock: true, State: BoardCellState.Movable };

            return UniTask.FromResult(result);
        }
    }
}
