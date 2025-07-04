// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Api.Game;
using MergeGame.Core.Extensions;
using MergeGame.Infrastructure.Extensions;
using VContainer;
using VExtensions.Mediator;

namespace MergeGame.Api.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterMergeGame(this IContainerBuilder builder)
        {
            builder.RegisterRepositories();
            builder.RegisterMediator(mediator => { mediator.RegisterCommands(); });
            builder.RegisterCore();
            builder.Register<GameController>(Lifetime.Singleton).AsSelf();
        }
    }
}
