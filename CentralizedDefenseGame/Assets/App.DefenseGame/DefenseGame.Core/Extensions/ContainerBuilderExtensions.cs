// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DefenseGame.Core.Features.GameSessions;
using VContainer;
using VExtensions.Mediator.Abstractions;

namespace DefenseGame.Core.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterCore(this IContainerBuilder builder, IMediatorBuilder mediator)
        {
            builder.RegisterGameSessions(mediator);
        }
    }
}
