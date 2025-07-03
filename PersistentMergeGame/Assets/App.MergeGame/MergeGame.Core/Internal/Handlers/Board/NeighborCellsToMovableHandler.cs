// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Data;
using MergeGame.Core.Internal.Extensions;
using MergeGame.Core.Internal.Managers;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board;

internal class NeighborCellsToMovableHandler
    : ICommandHandler<NeighborCellsToMovableCommand, NeighborCellsToMovableResult>
{
    private readonly GameManager _manager;

    public NeighborCellsToMovableHandler(GameManager manager)
    {
        _manager = manager;
    }

    public UniTask<NeighborCellsToMovableResult> ExecuteAsync(
        NeighborCellsToMovableCommand command,
        CancellationToken ct
    )
    {
        var board = _manager.GetBoardOrThrow(command.SessionId);

        List<BoardCell> updatedCells = new();

        foreach (Entities.BoardCell neighborCell in board.GetNeighborCells(command.Position))
        {
            if (neighborCell.ChangeMovable())
            {
                updatedCells.Add(BoardCell.FromEntity(neighborCell));
            }
        }

        return UniTask.FromResult(new NeighborCellsToMovableResult
        {
            UpdatedCells = updatedCells.Any() ? updatedCells.ToArray() : Array.Empty<BoardCell>()
        });
    }
}
