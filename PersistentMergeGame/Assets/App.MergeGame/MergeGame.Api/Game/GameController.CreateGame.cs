// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using MergeGame.Api.Game.CreateGame;
using MergeGame.Contracts.Board;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Commands.GameSession;
using MergeGame.Core.Extensions;

namespace MergeGame.Api.Game
{
    public partial class GameController
    {
        public async UniTask<FastResult<CreateGameResponse>> CreateGame(CancellationToken ct = default)
        {
            var sessionResult = await _mediator.ExecuteCreateStartingGameSession(
                new CreateStartingGameSessionCommand(),
                ct);

            if (sessionResult.IsError(out FastResult<CreateGameResponse> sessionFail))
            {
                return sessionFail;
            }

            var sessionId = sessionResult.Value;

            var boardSizeResult = await _mediator.ExecuteGetBoardSize(
                new GetBoardSizeCommand() { SessionId = sessionId }, ct
            );
            if (boardSizeResult.IsError(out FastResult<CreateGameResponse> sizeFail))
            {
                return sizeFail;
            }

            (int width, int height) = boardSizeResult.Value;
            var boardCellsResult = await _mediator.ExecuteGetBoardCells(
                new GetBoardCellsCommand() { SessionId = sessionId }, ct
            );

            if (boardCellsResult.IsError(out FastResult<CreateGameResponse> cellsFail))
            {
                return cellsFail;
            }

            IBoardCell[] boardCells = boardCellsResult.Value.Cast<IBoardCell>().ToArray();
            return FastResult<CreateGameResponse>.Ok(new CreateGameResponse(sessionId, width, height, boardCells));
        }
    }
}
