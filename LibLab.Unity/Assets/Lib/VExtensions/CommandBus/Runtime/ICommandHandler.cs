// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VitalRouter;

namespace VExtensions.CommandBus
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Void> where TCommand : ICommandBus
    {
        new UniTask ExecuteAsync(TCommand command, CancellationToken ct);

        async UniTask<Void> ICommandHandler<TCommand, Void>.ExecuteAsync(TCommand command, CancellationToken ct)
        {
            await ExecuteAsync(command, ct);
            return Void._instance;
        }
    }

    public interface ICommandHandler<in TCommand, TResult> : ICommandHandler where TCommand : ICommand<TResult>
    {
        UniTask<TResult> ExecuteAsync(TCommand command, CancellationToken ct);
    }

    public abstract class CommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private TResult? _result;

        public TResult Result
        {
            get
            {
                if (_result == null)
                {
                    throw new InvalidOperationException(
                        "Result is not set. Ensure that ExecuteAsync has been called before accessing Result.");
                }

                var result = _result;
                _result = default; // Clear the result after accessing it to prevent reuse.
                return result;
            }
        }

        public async UniTask On(TCommand command, PublishContext context)
        {
            _result = await ExecuteAsync(command, context.CancellationToken);
        }

        public abstract UniTask<TResult> ExecuteAsync(TCommand command, CancellationToken ct);
    }
}
