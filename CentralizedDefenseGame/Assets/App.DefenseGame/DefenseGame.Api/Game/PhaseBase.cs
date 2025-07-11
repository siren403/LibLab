// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalRouter;

namespace DefenseGame.Api.Game
{
    public abstract class PhaseBase<TPhase> : IPhaseContext where TPhase : IPhaseContext
    {
        public ILogger Logger => _logger;
        protected GameContext Context { get; private set; }

        private readonly ILogger<BeginPhase> _logger;

        protected PhaseBase(GameContext context)
        {
            Context = context;
            _logger = Context.CreateLogger<BeginPhase>();
        }

        protected ValueTask PublishAsync<TCommand>(TCommand command, CancellationToken ct = default)
            where TCommand : struct, IPhaseCommand<TPhase>
        {
            return Context.PublishAsync<TPhase, TCommand>(command, ct);
        }

        protected IDisposable SubscribeAwait<TCommand>(PublishContinuation<TCommand> handler)
            where TCommand : struct, IPhaseCommand<TPhase>
        {
            return Context.SubscribeAwait<TPhase, TCommand>(handler);
        }
    }
}
