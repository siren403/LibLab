// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefenseGame.Core.Application.Commands;
using DefenseGame.Core.Internal.Entities;
using DefenseGame.Core.Internal.Handlers;
using GameKit.Common.Results;
using GameKit.GameSessions.VContainer;
using VContainer;
using VExtensions.Mediator.Abstractions;

namespace DefenseGame.Core.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterCore(this IContainerBuilder builder, IMediatorBuilder mediator)
        {
            builder.RegisterGameSessions<GameState>();
            mediator.RegisterCommand<
                CreateGameSessionCommand,
                CreateGameSessionHandler,
                FastResult<CreateGameSessionData>
            >();
        }
    }

    public static class MediatorExtensions
    {
        public static UniTask<FastResult<CreateGameSessionData>> ExecuteCreateGameSession(this IMediator mediator,
            CreateGameSessionCommand command,
            CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CreateGameSessionCommand, FastResult<CreateGameSessionData>>(command, ct);
        }
    }
}
