using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Data;
using MergeGame.Core.Internal.Extensions;
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
            var board = _manager.GetBoardOrThrow(command.SessionId);
            var result = board.GetCells()
                .Where(cell => cell.HasBlock)
                .Select(BoardCell.FromEntity).ToArray();

            return UniTask.FromResult(result);
        }
    }
}
