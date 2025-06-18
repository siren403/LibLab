// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using VitalRouter;

namespace VExtensions.CommandBus
{
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

            var router = _container.Resolve<Router>()!;

            TResult result = default!;
            {
                var registry = _container.Resolve<CommandHandlerRegistry>();
                registry.TryGetValue(command.GetType(), out object handler);
                using var subscription = router.SubscribeAwait<ICommand<TResult>>(async (cmd, ctx) =>
                {
                    UniTask<TResult> task = (UniTask<TResult>)handler.GetType().GetMethod("ExecuteAsync").Invoke(
                        handler, new object[]
                        {
                            cmd, ctx.CancellationToken
                        });
                    // result = await ((ICommandHandler<ICommand<TResult>, TResult>)handler).ExecuteAsync(cmd,
                    //     ctx.CancellationToken);
                    result = await task;
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
