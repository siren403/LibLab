// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Api.Game.CreateGame;
using MergeGame.Common;
using MergeGame.Contracts.Board;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Commands.GameSession;
using MergeGame.Core.Extensions;

namespace MergeGame.Api.Game
{
    public partial class GameController
    {
        public async UniTask<Result<CreateGameResponse>> CreateGame(CancellationToken ct = default)
        {
            var result = await _mediator.ExecuteCreateStartingGameSession(
                new CreateStartingGameSessionCommand(),
                ct);

            if (result is not Ok<Ulid> (var sessionId))
            {
                return Result<CreateGameResponse>.Error("Failed to create game session.", result.StatusCode);
            }

            (int width, int height) = await _mediator.ExecuteGetBoardSize(
                new GetBoardSizeCommand() { SessionId = sessionId }, ct
            );

            IBoardCell[] boardCells = await _mediator.ExecuteGetBoardCells(
                new GetBoardCellsCommand() { SessionId = sessionId }, ct
            );

            return Result<CreateGameResponse>.Ok(new CreateGameResponse(sessionId, width, height, boardCells));
        }
    }
}
