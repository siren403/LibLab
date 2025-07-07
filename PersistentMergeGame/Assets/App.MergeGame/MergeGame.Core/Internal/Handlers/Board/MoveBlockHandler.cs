// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Common.Results;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Internal.Extensions;
using MergeGame.Core.Internal.Managers;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board;

internal class MoveBlockHandler : ICommandHandler<MoveBlockCommand, Result<bool>>
{
    private readonly GameManager _manager;

    public MoveBlockHandler(GameManager manager)
    {
        _manager = manager;
    }

    public UniTask<Result<bool>> ExecuteAsync(MoveBlockCommand command, CancellationToken ct)
    {
        var board = _manager.GetBoardOrThrow(command.SessionId);
        var result = board.MoveBlock(command.FromPosition, command.ToPosition);
        if (result.IsError<bool>(out var fail))
        {
            return fail;
        }

        return Result<bool>.Ok(true);
    }
}
