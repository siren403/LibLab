// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Commands.GameSession;
using MergeGame.Core.Application.Data;
using MergeGame.Core.Internal.Handlers.Board;
using MergeGame.Core.Internal.Handlers.GameSession;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;
using Void = GameKit.Common.Results.Void;

namespace MergeGame.Core.Extensions
{
    public static class MediatorBuilderExtensions
    {
        public static void RegisterCommands(this IMediatorBuilder builder)
        {
            #region GameSession

            builder.RegisterCommand<CreateGameSessionCommand, CreateGameSessionHandler, FastResult<Ulid>>();
            builder.RegisterCommand<
                CreateStartingGameSessionCommand,
                CreateStartingGameSessionHandler,
                FastResult<Ulid>
            >();

            #endregion

            #region Board

            builder.RegisterCommand<GetBoardSizeCommand, GetBoardSizeHandler, FastResult<BoardSize>>();
            builder.RegisterCommand<GetBoardCellsCommand, GetBoardCellsHandler, FastResult<BoardCell[]>>();
            builder.RegisterCommand<CheckMovableCellCommand, CheckMovableCellHandler, FastResult<BlockId>>();
            builder.RegisterCommand<MergeBlockCommand, MergeBlockHandler, FastResult<MergeBlockData>>();
            builder.RegisterCommand<
                NeighborCellsToMovableCommand,
                NeighborCellsToMovableHandler,
                NeighborCellsToMovableResult
            >();
            builder.RegisterCommand<MoveBlockCommand, MoveBlockHandler, FastResult<Void>>();
            builder.RegisterCommand<CheckEmptyCellCommand, CheckEmptyCellHandler, FastResult<bool>>();

            #endregion
        }
    }

    public static partial class MediatorExtensions
    {
        #region GameSession

        public static UniTask<FastResult<Ulid>> ExecuteCreateGameSession(this IMediator mediator,
            CreateGameSessionCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CreateGameSessionCommand, FastResult<Ulid>>(command, ct);
        }

        public static UniTask<FastResult<Ulid>> ExecuteCreateStartingGameSession(
            this IMediator mediator, CreateStartingGameSessionCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CreateStartingGameSessionCommand, FastResult<Ulid>>(command, ct);
        }

        #endregion

        #region Board

        public static UniTask<FastResult<BoardSize>> ExecuteGetBoardSize(this IMediator mediator,
            GetBoardSizeCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<GetBoardSizeCommand, FastResult<BoardSize>>(command, ct);
        }

        public static UniTask<FastResult<BoardCell[]>> ExecuteGetBoardCells(this IMediator mediator,
            GetBoardCellsCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<GetBoardCellsCommand, FastResult<BoardCell[]>>(command, ct);
        }

        public static UniTask<FastResult<BlockId>> ExecuteCheckMovableCell(this IMediator mediator,
            CheckMovableCellCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CheckMovableCellCommand, FastResult<BlockId>>(command, ct);
        }

        public static UniTask<FastResult<MergeBlockData>> ExecuteMergeBlock(this IMediator mediator,
            MergeBlockCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<MergeBlockCommand, FastResult<MergeBlockData>>(command, ct);
        }

        public static UniTask<NeighborCellsToMovableResult> ExecuteNeighborCellsToMovable(
            this IMediator mediator, NeighborCellsToMovableCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<NeighborCellsToMovableCommand, NeighborCellsToMovableResult>(command, ct);
        }

        public static UniTask<FastResult<Void>> ExecuteMoveBlock(this IMediator mediator,
            MoveBlockCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<MoveBlockCommand, FastResult<Void>>(command, ct);
        }

        public static UniTask<FastResult<bool>> ExecuteCheckEmptyCell(this IMediator mediator,
            CheckEmptyCellCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CheckEmptyCellCommand, FastResult<bool>>(command, ct);
        }

        #endregion
    }
}
