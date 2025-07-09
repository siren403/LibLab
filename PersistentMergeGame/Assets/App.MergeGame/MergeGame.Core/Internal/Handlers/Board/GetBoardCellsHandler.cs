using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Data;
using MergeGame.Core.Internal.Extensions;
using MergeGame.Core.Internal.Managers;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board
{
    internal class GetBoardCellsHandler : ICommandHandler<GetBoardCellsCommand, FastResult<BoardCell[]>>
    {
        private readonly GameManager _manager;

        public GetBoardCellsHandler(GameManager manager)
        {
            _manager = manager;
        }

        public UniTask<FastResult<BoardCell[]>> ExecuteAsync(GetBoardCellsCommand command, CancellationToken ct)
        {
            var boardResult = _manager.GetBoardOrError(command.SessionId);
            if (boardResult.IsError(out FastResult<BoardCell[]> fail))
            {
                return fail;
            }

            var board = boardResult.Value;
            var result = board.GetCells()
                .Where(cell => cell.HasBlock)
                .Select(BoardCell.FromEntity)
                .ToArray();

            return FastResult<BoardCell[]>.Ok(result);
        }
    }
}
