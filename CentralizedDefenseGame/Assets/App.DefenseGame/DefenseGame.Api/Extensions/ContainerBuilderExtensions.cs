// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DefenseGame.Api.Game;
using DefenseGame.Core.Extensions;
using DefenseGame.Infrastructure.Extensions;
using VContainer;
using VExtensions.Mediator;

namespace DefenseGame.Api.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterDefenseGame(this IContainerBuilder builder)
        {
            builder.RegisterInMemoryRepositories();
            builder.RegisterMediator(builder.RegisterCore);
            builder.Register<GameController>(Lifetime.Singleton).AsSelf();
        }
    }
}
