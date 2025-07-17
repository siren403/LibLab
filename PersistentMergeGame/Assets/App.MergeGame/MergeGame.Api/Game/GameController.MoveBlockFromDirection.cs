using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using MergeGame.Api.Game.MoveBlock;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Extensions;
using MergeGame.Core.ValueObjects;
using UnityEngine;

namespace MergeGame.Api.Game
{
    public partial class GameController
    {
        public async UniTask<FastResult<MovedResponse>> MoveBlockFromDirection(
            Ulid sessionId,
            Vector2Int fromPosition,
            Vector2 dragStartWorldPosition,
            Vector2 dragEndWorldPosition,
            CancellationToken ct = default)
        {
            // 드래그 방향 계산
            var direction = Direction.FromCoordinates(
                dragStartWorldPosition.x, dragStartWorldPosition.y,
                dragEndWorldPosition.x, dragEndWorldPosition.y
            );

            // 방향 기반 이동 커맨드 실행
            var command = new MoveBlockFromDirectionCommand
            {
                SessionId = sessionId,
                FromPosition = new Position(fromPosition.x, fromPosition.y),
                Direction = direction
            };

            var result = await _mediator.ExecuteMoveBlockFromDirection(command, ct);

            if (result.IsError(out FastResult<MovedResponse> fail))
            {
                return fail;
            }

            var newPosition = result.Value;
            return FastResult<MovedResponse>.Ok(new MovedResponse(new Vector2Int(newPosition.X, newPosition.Y)));
        }
    }
}
