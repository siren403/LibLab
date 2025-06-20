// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Cysharp.Threading.Tasks;
using VContainer;
using VitalRouter.VContainer;

namespace VExtensions.CommandBus
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterCommandBus(this IContainerBuilder builder, Action<CommandBusBuilder> configuration)
        {
            AddCommandBus(builder);
            builder.RegisterVitalRouter(routing =>
            {
                var bus = new CommandBusBuilder(builder, routing);
                configuration(bus);
            });
            builder.RegisterBuildCallback(MapCommandBus);
        }

        private static void AddCommandBus(IContainerBuilder builder)
        {
            builder.Register<CommandHandlerRegistry>(Lifetime.Singleton).AsSelf();
        }

        private static void MapCommandBus(IObjectResolver container)
        {
            CommandExtensions._container = container;
        }

        public class CommandBusBuilder
        {
            private readonly IContainerBuilder _builder;
            private readonly RoutingBuilder _routing;

            public CommandBusBuilder(IContainerBuilder builder, RoutingBuilder routing)
            {
                _builder = builder;
                _routing = routing;
            }

            public void AddCommand<TCommand, THandler, TResult>()
                where TCommand : class, ICommand<TResult>
                where THandler : ICommandHandler<TCommand, TResult>
            {
                _builder.Register<THandler>(Lifetime.Singleton).AsSelf();
                _builder.RegisterBuildCallback(container =>
                {
                    var registry = container.Resolve<CommandHandlerRegistry>();
                    registry.TryAdd(typeof(TCommand),
                        (CommandHandlerExecutor<TResult>)(
                            static (container, cmd, ct) =>
                                container.Resolve<THandler>().ExecuteAsync(
                                    (TCommand)cmd, ct)));
                });
            }

            public void AddFilterCommand<TCommand, THandler, TResult>()
                where TCommand : class, ICommand<TResult>
                where THandler : CommandHandler<TCommand, TResult>
            {
                _routing.Map<THandler>();
                _builder.RegisterBuildCallback(container =>
                {
                    var registry = container.Resolve<CommandHandlerRegistry>();
                    registry.TryAdd(typeof(TCommand),
                        (CommandHandlerExecutor<TResult>)(
                            static (container, cmd, ct) =>
                            {
                                var result = container.Resolve<THandler>().Result;
                                return UniTask.FromResult(result);
                            }));
                });
            }

            public void AddCommand<TCommand, THandler>()
                where TCommand : class, ICommandWithoutResult
                where THandler : ICommandHandler<TCommand>
            {
                _builder.Register<THandler>(Lifetime.Singleton);
                _builder.RegisterBuildCallback(container =>
                {
                    var registry = container.Resolve<CommandHandlerRegistry>();
                    registry.TryAdd(typeof(TCommand),
                        (CommandHandlerExecutor)(
                            static (container, cmd, ct) =>
                                container.Resolve<THandler>().ExecuteAsync(
                                    (TCommand)cmd, ct)));
                });
            }
        }
    }
}
