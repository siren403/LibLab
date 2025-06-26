// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.Internal.Repositories;
using MergeGame.Infrastructure.Repositories;
using VContainer;

namespace MergeGame.Infrastructure
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterRepositories(this IContainerBuilder builder)
        {
            builder.Register<BlockRepository>(Lifetime.Singleton).As<IBlockRepository>();
            builder.Register<BoardLayoutRepository>(Lifetime.Singleton).As<IBoardLayoutRepository>();
        }
    }
}
