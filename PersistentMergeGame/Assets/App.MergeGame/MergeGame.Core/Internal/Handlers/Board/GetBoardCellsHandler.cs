using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Data;
using MergeGame.Core.Internal.Managers;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board
{
    internal class GetBoardCellsHandler : ICommandHandler<GetBoardCellsCommand, BoardCell[]>
    {
        private readonly GameManager _manager;

        public GetBoardCellsHandler(GameManager manager)
        {
            _manager = manager;
        }

        public UniTask<BoardCell[]> ExecuteAsync(GetBoardCellsCommand command, CancellationToken ct)
        {
            (bool isSuccess, Entities.GameSession session, string? error) = _manager.GetSession(command.SessionId);
            if (!isSuccess)
            {
                return UniTask.FromResult(Array.Empty<BoardCell>());
            }

            var board = _manager.GetBoard(session);
            var result = board.GetCells()
                .Where(cell => cell.HasBlock)
                .Select(BoardCell.FromEntity).ToArray();

            return UniTask.FromResult(result);
        }
    }
}
