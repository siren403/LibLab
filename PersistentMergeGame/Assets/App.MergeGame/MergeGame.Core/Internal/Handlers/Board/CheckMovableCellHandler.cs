// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Common.Results;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Enums;
using MergeGame.Core.Internal.Extensions;
using MergeGame.Core.Internal.Managers;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board
{
    internal class CheckMovableCellHandler : ICommandHandler<CheckMovableCellCommand, Result<BlockId>>
    {
        private readonly GameManager _manager;

        public CheckMovableCellHandler(GameManager manager)
        {
            _manager = manager;
        }

        public UniTask<Result<BlockId>> ExecuteAsync(CheckMovableCellCommand command,
            CancellationToken ct)
        {
            var result = _manager.GetBoardOrError(command.SessionId);
            if (result.IsError(out Result<BlockId> fail))
            {
                return fail;
            }

            var board = result.Value;
            var cell = board.GetCell(command.Position);

            if (cell.TryGetBlockId(out var blockId) && cell.State == BoardCellState.Movable)
            {
                return Result<BlockId>.Ok(blockId);
            }

            return Result<BlockId>.Fail(ErrorCode.CannotMovableCell);
        }
    }
}
