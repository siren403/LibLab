using System;
using System.Threading;
using MergeGame.Core.Internal.Extensions;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.GameSession;
using MergeGame.Common.Results;
using MergeGame.Core.Internal.Managers;
using MergeGame.Core.Internal.Repositories;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.GameSession
{
    internal class CreateStartingGameSessionHandler : ICommandHandler<CreateStartingGameSessionCommand, Result<Ulid>>
    {
        private readonly GameManager _manager;
        private readonly IBoardLayoutRepository _repository;

        public CreateStartingGameSessionHandler(GameManager manager, IBoardLayoutRepository repository)
        {
            _manager = manager;
            _repository = repository;
        }

        public async UniTask<Result<Ulid>> ExecuteAsync(CreateStartingGameSessionCommand command,
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
                    return Result<Ulid>.Fail(
                        $"Failed to place block {spec.BlockId} at position {spec.Position} on the board {board.Id}.");
                }
            }

            return Result<Ulid>.Ok(session.Id);
        }
    }
}
