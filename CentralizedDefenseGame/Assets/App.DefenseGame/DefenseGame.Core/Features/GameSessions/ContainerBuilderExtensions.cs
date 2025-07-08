// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefenseGame.Common.Results;
using VContainer;
using VExtensions.Mediator.Abstractions;

namespace DefenseGame.Core.Features.GameSessions
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterGameSessions(this IContainerBuilder builder, IMediatorBuilder mediator)
        {
            builder.Register<GameSessionManager>(Lifetime.Singleton).AsSelf();
            mediator.RegisterCommand<CreateGameSessionCommand, CreateGameSessionHandler, Result<Ulid>>();
        }
    }

    public static class MediatorExtensions
    {
        public static UniTask<Result<Ulid>> ExecuteCreateGameSession(this IMediator mediator,
            CreateGameSessionCommand command, CancellationToken ct = default)
        {
            return mediator.ExecuteAsync<CreateGameSessionCommand, Result<Ulid>>(command, ct);
        }
    }
}
