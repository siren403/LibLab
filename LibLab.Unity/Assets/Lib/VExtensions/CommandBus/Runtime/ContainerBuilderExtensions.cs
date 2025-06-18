// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using VContainer;
using VitalRouter.VContainer;

namespace VExtensions.CommandBus
{
    public static class ContainerBuilderExtensions
    {
        public static void UseCommandBus(this IContainerBuilder builder)
        {
            AddCommandBus(builder);
            builder.RegisterBuildCallback(MapCommandBus);
        }

        private static void AddCommandBus(IContainerBuilder builder)
        {
            builder.RegisterVitalRouter(routing => { });
        }

        private static void MapCommandBus(IObjectResolver container)
        {
            CommandExtensions._container = container;
        }
    }
}
