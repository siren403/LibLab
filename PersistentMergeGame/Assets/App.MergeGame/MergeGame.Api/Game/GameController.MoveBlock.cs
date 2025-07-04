// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Api.Extensions;
using MergeGame.Api.Game.MoveBlock;
using MergeGame.Common;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Extensions;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Api.Game
{
    public partial class GameController
    {
        public async UniTask<MoveBlockResult> MoveBlock(
            Ulid sessionId,
            MoveBlockRequest request,
            CancellationToken ct = default
        )
        {
            bool emptyCell = await _mediator.ExecuteCheckEmptyCell(
                new CheckEmptyCellCommand() { SessionId = sessionId, Position = request.ToPosition.ToValue() }, ct);

            if (emptyCell)
            {
                bool ok = await _mediator.ExecuteMoveBlock(
                    new MoveBlockCommand()
                    {
                        SessionId = sessionId,
                        FromPosition = request.FromPosition.ToValue(),
                        ToPosition = request.ToPosition.ToValue()
                    }, ct);

                if (ok)
                {
                    return new MovedResult(request.ToPosition);
                }
            }
            else
            {
                var mergeResult = await _mediator.ExecuteMergeBlock(
                    new MergeBlockCommand()
                    {
                        SessionId = sessionId,
                        FromPosition = new Position(request.FromPosition.x, request.FromPosition.y),
                        ToPosition = new Position(request.ToPosition.x, request.ToPosition.y)
                    }, ct);

                if (mergeResult is not Ok<MergeBlockData> (var (fromCell, toCell, spawnedCell)))
                {
                    return MoveBlockResult.Error("Failed to move block.");
                }

                var toMovables = await _mediator.ExecuteNeighborCellsToMovable(
                    new NeighborCellsToMovableCommand()
                    {
                        SessionId = sessionId, Position = new Position(request.ToPosition.x, request.ToPosition.y)
                    }, ct);

                return new MergedResult(new MergedResponse(fromCell, toCell, spawnedCell, toMovables.UpdatedCells));
            }

            return MoveBlockResult.Error("Failed to move block.");
        }
    }
}
