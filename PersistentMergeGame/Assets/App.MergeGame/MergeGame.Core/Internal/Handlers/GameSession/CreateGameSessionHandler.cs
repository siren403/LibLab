// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.GameSession;
using MergeGame.Core.Internal.Managers;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.GameSession
{
    internal class CreateGameSessionHandler : ICommandHandler<CreateGameSessionCommand, CreateGameSessionResult>
    {
        private readonly GameManager _manager;

        public CreateGameSessionHandler(GameManager manager)
        {
            _manager = manager;
        }

        public UniTask<CreateGameSessionResult> ExecuteAsync(CreateGameSessionCommand command,
            CancellationToken ct)
        {
            int width = command.Width;
            int height = command.Height;
            var session = _manager.CreateGameSession(width, height);
            var result = new CreateGameSessionResult(true, session.Id);
            return UniTask.FromResult(result);
        }
    }
}
