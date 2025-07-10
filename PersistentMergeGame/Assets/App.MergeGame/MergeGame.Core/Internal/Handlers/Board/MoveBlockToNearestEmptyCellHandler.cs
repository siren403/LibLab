using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Internal.Managers;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board
{
    internal class
        MoveBlockToNearestEmptyCellHandler : ICommandHandler<MoveBlockToNearestEmptyCellCommand, FastResult<Position>>
    {
        private readonly GameManager _manager;

        public MoveBlockToNearestEmptyCellHandler(GameManager manager)
        {
            _manager = manager;
        }

        public UniTask<FastResult<Position>> ExecuteAsync(MoveBlockToNearestEmptyCellCommand command,
            CancellationToken cancellationToken)
        {
            var sessionResult = _manager.GetSession(command.SessionId);
            if (sessionResult.IsError(out FastResult<Position> fail))
            {
                return fail;
            }

            var board = _manager.GetBoard(sessionResult.Value);

            var findResult = board.FindNearestEmptyCell(command.FromPosition, command.ToPosition);
            if (findResult.IsError(out FastResult<Position> findFail))
            {
                return findFail;
            }

            var nearestPosition = findResult.Value.Position;
            var moveResult = board.MoveBlock(command.FromPosition, nearestPosition);

            return moveResult.IsError(out FastResult<Position> moveFail)
                ? moveFail
                : FastResult<Position>.Ok(nearestPosition);
        }
    }
}
