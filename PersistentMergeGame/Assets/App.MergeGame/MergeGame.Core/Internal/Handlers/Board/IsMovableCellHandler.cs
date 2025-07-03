// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Enums;
using MergeGame.Core.Internal.Managers;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board
{
    internal class IsMovableCellHandler : ICommandHandler<IsMovableCellCommand, IsMovableCellResult>
    {
        private readonly GameManager _manager;

        public IsMovableCellHandler(GameManager manager)
        {
            _manager = manager;
        }

        public UniTask<IsMovableCellResult> ExecuteAsync(IsMovableCellCommand command, CancellationToken ct)
        {
            (bool isSuccess, Entities.GameSession session, _) = _manager.GetSession(command.SessionId);
            if (!isSuccess)
            {
                throw new InvalidOperationException($"Session with ID {command.SessionId} not found.");
            }

            var board = _manager.GetBoard(session);
            var cell = board.GetCell(command.Position);

            if (cell.TryGetBlockId(out var blockId) && cell.State == BoardCellState.Movable)
            {
                return UniTask.FromResult(new IsMovableCellResult() { BlockId = blockId, IsMovable = true });
            }

            return UniTask.FromResult(new IsMovableCellResult()
            {
                BlockId = BlockId.Invalid,
                IsMovable = false
            });
        }
    }
}
