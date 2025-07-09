// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using MergeGame.Api.Extensions;
using MergeGame.Api.Game.MoveBlock;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Data;
using MergeGame.Core.Extensions;
using MergeGame.Core.ValueObjects;
using Void = GameKit.Common.Results.Void;

namespace MergeGame.Api.Game
{
    public partial class GameController
    {
        public async UniTask<FastResult<MoveBlockResponse>> MoveBlock(
            Ulid sessionId,
            MoveBlockRequest request,
            CancellationToken ct = default
        )
        {
            var emptyCellResult = await _mediator.ExecuteCheckEmptyCell(
                new CheckEmptyCellCommand() { SessionId = sessionId, Position = request.ToPosition.ToValue() }, ct);

            if (emptyCellResult.IsError(out FastResult<MoveBlockResponse> emptyCellFail))
            {
                return emptyCellFail;
            }

            bool emptyCell = emptyCellResult.Value;
            if (emptyCell)
            {
                FastResult<Void> ok = await _mediator.ExecuteMoveBlock(
                    new MoveBlockCommand()
                    {
                        SessionId = sessionId,
                        FromPosition = request.FromPosition.ToValue(),
                        ToPosition = request.ToPosition.ToValue()
                    }, ct);

                return ok.IsError(out FastResult<MoveBlockResponse> moveFail)
                    ? moveFail
                    : FastResult<MoveBlockResponse>.Ok(new MovedResponse(request.ToPosition));
            }

            var mergeResult = await _mediator.ExecuteMergeBlock(
                new MergeBlockCommand()
                {
                    SessionId = sessionId,
                    FromPosition = new Position(request.FromPosition.x, request.FromPosition.y),
                    ToPosition = new Position(request.ToPosition.x, request.ToPosition.y)
                }, ct);

            if (mergeResult.IsError(out FastResult<MoveBlockResponse> mergeFail))
            {
                return mergeFail;
            }

            var toMovables = await _mediator.ExecuteNeighborCellsToMovable(
                new NeighborCellsToMovableCommand()
                {
                    SessionId = sessionId, Position = new Position(request.ToPosition.x, request.ToPosition.y)
                }, ct);

            (BoardCell fromCell, BoardCell toCell, BoardCell spawnedCell) = mergeResult.Value;
            return FastResult<MoveBlockResponse>.Ok(new MergedResponse(fromCell, toCell, spawnedCell,
                toMovables.UpdatedCells));
        }
    }
}
