// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using VContainer;
using VitalRouter;

namespace VExtensions.CommandBus
{
    public delegate UniTask<TResult> CommandHandlerExecutor<TResult>(IObjectResolver container, object command,
        CancellationToken token);

    public delegate UniTask CommandHandlerExecutor(IObjectResolver container, object command,
        CancellationToken token);

    public static class CommandExtensions
    {
        internal static IObjectResolver? _container;

        public static async UniTask<TResult> ExecuteAsync<TCommand, TResult>(
            this TCommand command,
            CancellationToken ct = default
        ) where TCommand : ICommand<TResult>
        {
            if (_container == null)
            {
                throw new InvalidOperationException(
                    "CommandExtensions._container is not set. Please ensure that the VContainer is properly configured and the container is assigned before using command execution methods."
                );
            }

            var router = _container.Resolve<Router>();

            TResult result = default!;
            {
                var registry = _container.Resolve<CommandHandlerRegistry>();
                bool hasExecutor = registry.TryGetValue(command.GetType(), out object executor);
                if (!hasExecutor)
                {
                    throw new InvalidOperationException(
                        $"No command handler registered for command type {command.GetType().Name}. Please ensure that a handler is registered in the CommandHandlerRegistry."
                    );
                }

                await router.PublishAsync(command, ct);
                result = await ((CommandHandlerExecutor<TResult>)executor!).Invoke(
                    _container, command, ct
                );

                if (result == null)
                {
                    throw new InvalidOperationException(
                        $"Command handler for command type {command.GetType().Name} returned null. Ensure that the handler is implemented correctly and returns a valid result."
                    );
                }
            }

            if (ct.IsCancellationRequested)
            {
                throw new OperationCanceledException("Command execution was canceled.");
            }

            return result;
        }

        public static UniTask<TResult> ExecuteAsync<TResult>(
            this ICommand<TResult> command,
            CancellationToken ct = default
        )
        {
            return ExecuteAsync<ICommand<TResult>, TResult>(command, ct);
        }

        public static async UniTask ExecuteAsync<TCommand>(
            this TCommand command,
            CancellationToken ct = default
        ) where TCommand : ICommandWithoutResult
        {
            if (_container == null)
            {
                throw new InvalidOperationException(
                    "CommandExtensions._container is not set. Please ensure that the VContainer is properly configured and the container is assigned before using command execution methods."
                );
            }

            var router = _container.Resolve<Router>();

            var registry = _container.Resolve<CommandHandlerRegistry>();
            bool hasExecutor = registry.TryGetValue(command.GetType(), out object executor);
            if (!hasExecutor)
            {
                throw new InvalidOperationException(
                    $"No command handler registered for command type {command.GetType().Name}. Please ensure that a handler is registered in the CommandHandlerRegistry."
                );
            }

            bool isCalled = false;
            using var subscription = router.SubscribeAwait<TCommand>(async (cmd, ctx) =>
            {
                isCalled = true;
                await ((CommandHandlerExecutor)executor!).Invoke(_container, cmd, ctx.CancellationToken);
            });

            await router.PublishAsync(command, ct);

            if (!isCalled)
            {
                throw new InvalidOperationException(
                    $"No command handler was called for command type {command.GetType().Name}. Please ensure that a handler is registered and that the command is published correctly."
                );
            }

            if (ct.IsCancellationRequested)
            {
                throw new OperationCanceledException("Command execution was canceled.");
            }
        }
    }
}
