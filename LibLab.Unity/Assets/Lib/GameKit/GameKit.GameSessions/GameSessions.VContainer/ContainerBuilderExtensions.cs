// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using GameKit.Common.Results;
using GameKit.GameSessions.Core;
using VContainer;
using VExtensions.Mediator;

namespace GameKit.GameSessions.VContainer
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterGameSessions<TGameState>(this IContainerBuilder builder)
            where TGameState : IGameState
        {
            builder.Register<GameSessionManager<TGameState>>(Lifetime.Singleton).AsSelf();
            builder.RegisterMediator(mediator =>
            {
                mediator.RegisterCommand<
                    CreateGameSessionCommand<TGameState>,
                    CreateGameSessionHandler<TGameState>,
                    Result<Ulid>
                >();
            });
        }
    }
}
