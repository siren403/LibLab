// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Commands.GameSession;
using MergeGame.Core.Application.Data;
using MergeGame.Common;
using MergeGame.Core.Internal.Handlers.Board;
using MergeGame.Core.Internal.Handlers.GameSession;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Extensions
{
    public static class MediatorBuilderExtensions
    {
        public static void RegisterCommands(this IMediatorBuilder builder)
        {
            #region GameSession

            builder.RegisterCommand<CreateGameSessionCommand, CreateGameSessionHandler, Result<Ulid>>();
            builder.RegisterCommand<
                CreateStartingGameSessionCommand,
                CreateStartingGameSessionHandler,
                Result<Ulid>
            >();

            #endregion

            #region Board

            builder.RegisterCommand<GetBoardSizeCommand, GetBoardSizeHandler, BoardSize>();
            builder.RegisterCommand<GetBoardCellsCommand, GetBoardCellsHandler, BoardCell[]>();
            builder.RegisterCommand<CheckMovableCellCommand, CheckMovableCellHandler, CheckMovableCellResult>();
            builder.RegisterCommand<MergeBlockCommand, MergeBlockHandler, Result<MergeBlockData>>();
            builder.RegisterCommand<
                NeighborCellsToMovableCommand,
                NeighborCellsToMovableHandler,
                NeighborCellsToMovableResult
            >();
            builder.RegisterCommand<MoveBlockCommand, MoveBlockHandler, bool>();
            builder.RegisterCommand<CheckEmptyCellCommand, CheckEmptyCellHandler, bool>();

            #endregion
        }
    }

    public static class MediatorExtensions
    {
        #region GameSession

        public static UniTask<Result<Ulid>> ExecuteCreateGameSession(this IMediator mediator,
            CreateGameSessionCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CreateGameSessionCommand, Result<Ulid>>(command, ct);
        }

        public static UniTask<Result<Ulid>> ExecuteCreateStartingGameSession(
            this IMediator mediator, CreateStartingGameSessionCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CreateStartingGameSessionCommand, Result<Ulid>>(command, ct);
        }

        #endregion

        #region Board

        public static UniTask<BoardSize> ExecuteGetBoardSize(this IMediator mediator,
            GetBoardSizeCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<GetBoardSizeCommand, BoardSize>(command, ct);
        }

        public static UniTask<BoardCell[]> ExecuteGetBoardCells(this IMediator mediator,
            GetBoardCellsCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<GetBoardCellsCommand, BoardCell[]>(command, ct);
        }

        public static UniTask<CheckMovableCellResult> ExecuteCheckMovableCell(this IMediator mediator,
            CheckMovableCellCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CheckMovableCellCommand, CheckMovableCellResult>(command, ct);
        }

        public static UniTask<Result<MergeBlockData>> ExecuteMergeBlock(this IMediator mediator,
            MergeBlockCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<MergeBlockCommand, Result<MergeBlockData>>(command, ct);
        }

        public static UniTask<NeighborCellsToMovableResult> ExecuteNeighborCellsToMovable(
            this IMediator mediator, NeighborCellsToMovableCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<NeighborCellsToMovableCommand, NeighborCellsToMovableResult>(command, ct);
        }

        public static UniTask<bool> ExecuteMoveBlock(this IMediator mediator,
            MoveBlockCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<MoveBlockCommand, bool>(command, ct);
        }

        public static UniTask<bool> ExecuteCheckEmptyCell(this IMediator mediator,
            CheckEmptyCellCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CheckEmptyCellCommand, bool>(command, ct);
        }

        #endregion
    }
}
