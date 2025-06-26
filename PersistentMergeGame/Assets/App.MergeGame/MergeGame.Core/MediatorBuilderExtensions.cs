// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Commands.GameSession;
using MergeGame.Core.Internal.Handlers.Board;
using MergeGame.Core.Internal.Handlers.GameSession;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core
{
    public static class MediatorBuilderExtensions
    {
        public static void RegisterCommands(this IMediatorBuilder builder)
        {
            builder.RegisterCommand<CreateGameSessionCommand, CreateGameSessionHandler, CreateGameSessionResult>();
            builder.RegisterCommand<
                CreateStartingGameSessionCommand,
                CreateStartingGameSessionHandler,
                CreateGameSessionResult
            >();

            builder.RegisterCommand<GetBoardSizeCommand, GetBoardSizeHandler, BoardSize>();
        }
    }

    public static class MediatorExtensions
    {
        public static UniTask<CreateGameSessionResult> ExecuteCreateGameSession(this IMediator mediator,
            CreateGameSessionCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CreateGameSessionCommand, CreateGameSessionResult>(command, ct);
        }

        public static UniTask<CreateGameSessionResult> ExecuteCreateStartingGameSession(
            this IMediator mediator, CreateStartingGameSessionCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CreateStartingGameSessionCommand, CreateGameSessionResult>(command, ct);
        }

        public static UniTask<BoardSize> ExecuteGetBoardSize(this IMediator mediator,
            GetBoardSizeCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<GetBoardSizeCommand, BoardSize>(command, ct);
        }
    }
}
