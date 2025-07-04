// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.GameSession;
using MergeGame.Common;
using MergeGame.Core.Internal.Managers;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.GameSession
{
    internal class CreateGameSessionHandler : ICommandHandler<CreateGameSessionCommand, Result<Ulid>>
    {
        private readonly GameManager _manager;

        public CreateGameSessionHandler(GameManager manager)
        {
            _manager = manager;
        }

        public UniTask<Result<Ulid>> ExecuteAsync(CreateGameSessionCommand command,
            CancellationToken ct)
        {
            int width = command.Width;
            int height = command.Height;
            var session = _manager.CreateGameSession(width, height);
            var result = Result<Ulid>.Ok(session.Id);
            return UniTask.FromResult<Result<Ulid>>(result);
        }
    }
}
