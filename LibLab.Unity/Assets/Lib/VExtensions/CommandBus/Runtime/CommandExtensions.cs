// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using VitalRouter;

namespace VExtensions.CommandBus
{
    public delegate UniTask<TResult> CommandHandlerExecutor<TResult>(IObjectResolver container, object command,
        CancellationToken token);
    public static class CommandExtensions
    {
        internal static IObjectResolver? _container;


        public static async UniTask<TResult> ExecuteAsync<TResult>(
            this ICommand<TResult> command,
            CancellationToken ct = default
        )
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

                using var subscription = router.SubscribeAwait<ICommand<TResult>>(async (cmd, ctx) =>
                {
                    result = await ((CommandHandlerExecutor<TResult>)executor!).Invoke(
                        _container,
                        cmd,
                        ctx.CancellationToken
                    );
                });

                await router.PublishAsync(command, ct);
            }

            if (ct.IsCancellationRequested)
            {
                throw new OperationCanceledException("Command execution was canceled.");
            }

            return result;
        }
    }
}
