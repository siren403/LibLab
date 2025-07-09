// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Internal.Extensions;
using MergeGame.Core.Internal.Managers;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board;

internal class CheckEmptyCellHandler : ICommandHandler<CheckEmptyCellCommand, FastResult<bool>>
{
    private readonly GameManager _manager;

    public CheckEmptyCellHandler(GameManager manager)
    {
        _manager = manager;
    }

    public UniTask<FastResult<bool>> ExecuteAsync(CheckEmptyCellCommand command, CancellationToken ct)
    {
        var result = _manager.GetBoardOrError(command.SessionId);

        if (result.IsError(out FastResult<bool> fail))
        {
            return fail;
        }

        var board = result.Value;
        var cell = board.GetCell(command.Position);
        return FastResult<bool>.Ok(!cell.HasBlock);
    }
}
