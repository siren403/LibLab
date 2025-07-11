// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using Microsoft.Extensions.Logging;
using R3;
using VitalRouter;
using Void = GameKit.Common.Results.Void;

namespace DefenseGame.Api.Game
{
    public class GameContext : IDisposable
    {
        private readonly ILoggerFactory _loggerFactory;
        public Ulid SessionId { get; private set; }
        public bool IsStarted { get; private set; } = false;
        public bool IsPlaying { get; internal set; } = false;

        public FastResult<Void> GameResult { get; private set; } = FastResult.Ok;
        public readonly CancellationToken CancellationToken;

        private readonly Router _router;
        private readonly CancellationTokenSource _internalTokenSource;
        private DisposableBag _disposables;

        public GameContext(Ulid sessionId, Router router, ILoggerFactory loggerFactory,
            CancellationToken externalToken = default)
        {
            _loggerFactory = loggerFactory;
            SessionId = sessionId;
            _router = router;

            _internalTokenSource = new CancellationTokenSource().AddTo(ref _disposables);
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                _internalTokenSource.Token,
                externalToken
            ).AddTo(ref _disposables);
            CancellationToken = linkedTokenSource.Token;
        }

        public UniTask<BeginPhase> BeginAsync(CancellationToken ct)
        {
            IsStarted = true;
            IsPlaying = true;
            var beginPhase = new BeginPhase(this);
            return UniTask.FromResult(beginPhase);
        }

        public ILogger<T> CreateLogger<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }

        internal IDisposable SubscribeAwait<TPhase, TCommand>(PublishContinuation<TCommand> handler)
            where TPhase : IPhaseContext
            where TCommand : struct, IPhaseCommand<TPhase>
        {
            return _router.SubscribeAwait(handler, CommandOrdering.Drop);
        }

        internal ValueTask PublishAsync<TPhase, TCommand>(
            TCommand command,
            CancellationToken ct = default)
            where TPhase : IPhaseContext
            where TCommand : struct, IPhaseCommand<TPhase>
        {
            return _router.PublishAsync(command, ct);
        }

        public void Cancel(FastResult<Void> fail)
        {
            GameResult = fail;
            _internalTokenSource.Cancel();
            CancellationToken.ThrowIfCancellationRequested();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
