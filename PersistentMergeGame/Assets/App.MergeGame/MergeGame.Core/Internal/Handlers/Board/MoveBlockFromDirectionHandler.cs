using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Internal.Extensions;
using MergeGame.Core.Internal.Managers;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board
{
    internal class MoveBlockFromDirectionHandler : ICommandHandler<MoveBlockFromDirectionCommand, FastResult<Position>>
    {
        private readonly GameManager _manager;

        public MoveBlockFromDirectionHandler(GameManager manager)
        {
            _manager = manager;
        }

        public UniTask<FastResult<Position>> ExecuteAsync(MoveBlockFromDirectionCommand command, CancellationToken ct)
        {
            var boardResult = _manager.GetBoardOrError(command.SessionId);
            if (boardResult.IsError(out FastResult<Position> boardFail))
            {
                return UniTask.FromResult(boardFail);
            }

            var board = boardResult.Value;

            // 방향 기반으로 가장 가까운 빈 셀 찾기
            var findResult = board.FindNearestEmptyCellFromDirection(command.FromPosition, command.Direction);
            if (findResult.IsError(out FastResult<Position> findFail))
            {
                return UniTask.FromResult(findFail);
            }

            var targetPosition = findResult.Value.Position;

            // 실제 블록 이동
            var moveResult = board.MoveBlock(command.FromPosition, targetPosition);
            if (moveResult.IsError(out FastResult<Position> moveFail))
            {
                return UniTask.FromResult(moveFail);
            }

            return UniTask.FromResult(FastResult<Position>.Ok(targetPosition));
        }
    }
}