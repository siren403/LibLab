using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        public async UniTask<CreateGameResponse> CreateGame(CreateGameRequest request, CancellationToken ct = default)
        {
            (bool isSuccess, Ulid sessionId, _) = await _mediator.ExecuteCreateStartingGameSession(
                new CreateStartingGameSessionCommand(),
                ct);

            if (!isSuccess)
            {
                return CreateGameResponse.Error();
            }

            (int width, int height) = await _mediator.ExecuteGetBoardSize(
                new GetBoardSizeCommand() { SessionId = sessionId }, ct
            );

            IBoardCell[] boardCells = await _mediator.ExecuteGetBoardCells(
                new GetBoardCellsCommand() { SessionId = sessionId }, ct
            );

            return CreateGameResponse.Ok(sessionId, width, height, boardCells);
        }

        public async UniTask<bool> IsMovableCell(Ulid sessionId, Vector2Int position, CancellationToken ct = default)
        {
            bool isMovable = await _mediator.ExecuteIsMovableCell(
                new IsMovableCellCommand() { SessionId = sessionId, Position = new Position(position.x, position.y) },
                ct);

            return isMovable;
        }
    }
}
