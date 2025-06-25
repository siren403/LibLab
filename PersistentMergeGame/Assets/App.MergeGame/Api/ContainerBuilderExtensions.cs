// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core;
using MergeGame.Infrastructure;
using VContainer;
using VExtensions.Mediator;

namespace MergeGame.Api
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterMergeGame(this IContainerBuilder builder)
        {
            builder.RegisterRepositories();
            builder.RegisterMediator(mediator => { mediator.RegisterCommands(); });

            builder.Register<MergeGameController>(Lifetime.Singleton).AsSelf();
        }
    }
}
