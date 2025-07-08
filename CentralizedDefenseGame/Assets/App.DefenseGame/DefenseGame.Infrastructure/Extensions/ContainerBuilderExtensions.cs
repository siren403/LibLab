// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DefenseGame.Infrastructure.Internal.Repositories.InMemory;
using DefenseGame.Internal.Repositories;
using VContainer;

namespace DefenseGame.Infrastructure.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterInMemoryRepositories(this IContainerBuilder builder)
        {
            builder.Register<InMemoryUnitRepository>(Lifetime.Singleton).As<IUnitRepository>();
            builder.Register<InMemoryUnitSpecRepository>(Lifetime.Singleton).As<IUnitSpecRepository>();
        }
    }
}
