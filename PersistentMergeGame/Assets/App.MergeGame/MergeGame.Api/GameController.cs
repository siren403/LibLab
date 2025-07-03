using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Api.Extensions;
using MergeGame.Contracts.Board;
using MergeGame.Api.Game;
using MergeGame.Core;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Commands.GameSession;
using MergeGame.Core.ValueObjects;
using UnityEngine;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Api
{
    public class GameController
    {
        private readonly IMediator _mediator;

        public GameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async UniTask<CreateGameResponse> CreateGame(CreateGameRequest request,
            CancellationToken ct = default)
        {
            (bool isSuccess, Ulid sessionId, _) = await _mediator.ExecuteCreateStartingGameSession(
                new CreateStartingGameSessionCommand(),
                ct);

            if (!isSuccess)
            {
                return CreateGameResponse.Error();
            }

            (int width, int height) = await _mediator.ExecuteGetBoardSize(
                new GetBoardSizeCommand()
                {
                    SessionId = sessionId
                }, ct
            );

            IBoardCell[] boardCells = await _mediator.ExecuteGetBoardCells(
                new GetBoardCellsCommand()
                {
                    SessionId = sessionId
                }, ct
            );

            return CreateGameResponse.Ok(sessionId, width, height, boardCells);
        }

        public async UniTask<(bool ok, long blockId)> CheckMovableCell(Ulid sessionId, Vector2Int position,
            CancellationToken ct = default)
        {
            try
            {
                var result = await _mediator.ExecuteCheckMovableCell(
                    new CheckMovableCellCommand()
                    {
                        SessionId = sessionId, Position = new Position(position.x, position.y)
                    },
                    ct);

                return (result.IsMovable, result.BlockId);
            }
            catch
            {
                return (false, BlockId.Invalid);
            }
        }

        /// <summary>
        /// TODO: MoveBlock으로 이름 변경 후 로직 변경
        /// ToCell이 비어있으면 MoveBlock, 아니면 MergeBlock으로 처리
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async UniTask<MergeBlockResponse> MergeBlock(Ulid sessionId, MergeBlockRequest request,
            CancellationToken ct = default)
        {
            try
            {
                (IBoardCell from, IBoardCell to, IBoardCell spawned) = await _mediator.ExecuteMergeBlock(
                    new MergeBlockCommand()
                    {
                        SessionId = sessionId,
                        FromPosition = new Position(request.FromPosition.x, request.FromPosition.y),
                        ToPosition = new Position(request.ToPosition.x, request.ToPosition.y)
                    }, ct);

                var toMovables = await _mediator.ExecuteNeighborCellsToMovable(
                    new NeighborCellsToMovableCommand()
                    {
                        SessionId = sessionId, Position = new Position(request.ToPosition.x, request.ToPosition.y)
                    }, ct);

                return new MergeBlockResponse(true, from, to, spawned, toMovables.UpdatedCells);
            }
            catch
            {
                return MergeBlockResponse.Error;
            }
        }

        public async UniTask<MoveBlockResponse> MoveBlock(Ulid sessionId, MoveBlockRequest request,
            CancellationToken ct = default)
        {
            try
            {
                bool emptyCell = await _mediator.ExecuteCheckEmptyCell(new CheckEmptyCellCommand()
                {
                    SessionId = sessionId, Position = request.ToPosition.ToValue()
                }, ct);

                if (emptyCell)
                {
                    bool ok = await _mediator.ExecuteMoveBlock(new MoveBlockCommand()
                    {
                        SessionId = sessionId,
                        FromPosition = request.FromPosition.ToValue(),
                        ToPosition = request.ToPosition.ToValue()
                    }, ct);

                    if (ok)
                    {
                        return new MovedResponse(0, request.ToPosition);
                    }
                }
                else
                {
                    (IBoardCell from, IBoardCell to, IBoardCell spawned) = await _mediator.ExecuteMergeBlock(
                        new MergeBlockCommand()
                        {
                            SessionId = sessionId,
                            FromPosition = new Position(request.FromPosition.x, request.FromPosition.y),
                            ToPosition = new Position(request.ToPosition.x, request.ToPosition.y)
                        }, ct);

                    var toMovables = await _mediator.ExecuteNeighborCellsToMovable(
                        new NeighborCellsToMovableCommand()
                        {
                            SessionId = sessionId,
                            Position = new Position(request.ToPosition.x, request.ToPosition.y)
                        }, ct);

                    return new MergedResponse(0, from, to, spawned, toMovables.UpdatedCells);
                }

                return MoveBlockResponse.Error;
            }
            catch
            {
                return MoveBlockResponse.Error;
            }
        }
    }

    public record MoveBlockRequest(Vector2Int FromPosition, Vector2Int ToPosition);

    public record MoveBlockResponse(int StatusCode) : Response(StatusCode)
    {
        public static readonly MoveBlockResponse Error = new(-1);
        public static readonly MoveBlockResponse Ok = new(0);
    }

    public record MovedResponse(int StatusCode, Vector2Int ToCell) : MoveBlockResponse(StatusCode);

    public record MergedResponse(
        int StatusCode,
        IBoardCell FromCell,
        IBoardCell ToCell,
        IBoardCell SpawnedCell,
        IReadOnlyList<IBoardCell> UpdatedCells
    ) : MoveBlockResponse(StatusCode);

}
