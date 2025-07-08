// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefenseGame.Common.Results;
using VExtensions.Mediator.Abstractions;

namespace DefenseGame.Core.Features.GameSessions
{
    internal class CreateGameSessionHandler : ICommandHandler<CreateGameSessionCommand, Result<Ulid>>
    {
        private readonly GameSessionManager _sessionManager;

        public CreateGameSessionHandler(GameSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public UniTask<Result<Ulid>> ExecuteAsync(CreateGameSessionCommand command,
            CancellationToken ct)
        {
            if (command.Radius <= 0)
            {
                return Result<Ulid>.Fail(
                    $"{nameof(GameSessions)} {nameof(CreateGameSessionCommand.Radius)} must be greater than zero.");
            }

            var session = _sessionManager.CreateGameSession(command.Radius);
            var result = Result<Ulid>.Ok(session.Id);
            return result;
        }
    }
}
