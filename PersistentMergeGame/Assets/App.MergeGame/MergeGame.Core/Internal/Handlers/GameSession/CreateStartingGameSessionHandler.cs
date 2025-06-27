// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using MergeGame.Core.Internal.Extensions;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.GameSession;
using MergeGame.Core.Internal.Managers;
using MergeGame.Core.Internal.Repositories;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.GameSession
{
    internal class CreateStartingGameSessionHandler
        : ICommandHandler<CreateStartingGameSessionCommand, CreateGameSessionResult>
    {
        private readonly GameManager _manager;
        private readonly IBoardLayoutRepository _repository;

        public CreateStartingGameSessionHandler(GameManager manager, IBoardLayoutRepository repository)
        {
            _manager = manager;
            _repository = repository;
        }

        public async UniTask<CreateGameSessionResult> ExecuteAsync(CreateStartingGameSessionCommand command,
            CancellationToken ct)
        {
            var layout = await _repository.GetStartingLayout(ct);
            var session = _manager.CreateGameSession(layout.Width, layout.Height);

            var board = _manager.GetBoard(session);
            foreach (BoardCellSpec spec in layout.Cells)
            {
                bool result = board.PlaceBlock(spec.Position, spec.BlockId, spec.State);
                if (!result)
                {
                    throw new InvalidOperationException(
                        $"Failed to place block {spec.BlockId} at position {spec.Position} on the board {board.Id}.");
                }
            }

            return new CreateGameSessionResult(true, session.Id);
        }
    }
}
