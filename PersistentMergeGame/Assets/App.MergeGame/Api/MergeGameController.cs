// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Commands.MergeBoards;
using MergeGame.Core.ValueObjects;
using UnityEngine;
using VExtensions.Mediator.Abstractions;
using VitalRouter;

namespace MergeGame.Api
{
    public class MergeGameController
    {
        private readonly IMediator _mediator;
        private readonly Router _router;

        public MergeGameController(IMediator mediator, Router router)
        {
            _mediator = mediator;
            _router = router;
        }

        public async UniTask<(int Width, int Height)> CreateBoard(CancellationToken ct)
        {
            await _mediator.ExecuteAsync(new CreateBoardCommand() { Width = 7, Height = 9 }, ct);

            var size = await _mediator.ExecuteAsync<GetBoardSizeCommand, (int Width, int Height)>(
                new GetBoardSizeCommand(), ct
            );

            return size;
        }

        public UniTask<(EntityId, Vector2Int)[]> GetBlocks(CancellationToken ct)
        {
            var blocks = new[]
            {
                (new EntityId(1), new Vector2Int(0, 0)), (new EntityId(2), new Vector2Int(1, 0)),
                (new EntityId(3), new Vector2Int(2, 0)), (new EntityId(4), new Vector2Int(3, 0)),
                (new EntityId(5), new Vector2Int(4, 0)), (new EntityId(6), new Vector2Int(5, 0)),
                (new EntityId(7), new Vector2Int(6, 0)),
                //
                (new EntityId(1), new Vector2Int(0, 2)), (new EntityId(2), new Vector2Int(1, 2)),
                (new EntityId(3), new Vector2Int(2, 2)), (new EntityId(4), new Vector2Int(3, 2)),
                (new EntityId(5), new Vector2Int(4, 2)), (new EntityId(6), new Vector2Int(5, 2)),
                (new EntityId(7), new Vector2Int(6, 2)),
                //
                (new EntityId(1), new Vector2Int(0, 4)), (new EntityId(2), new Vector2Int(1, 4)),
                (new EntityId(3), new Vector2Int(2, 4)), (new EntityId(4), new Vector2Int(3, 4)),
                (new EntityId(5), new Vector2Int(4, 4)), (new EntityId(6), new Vector2Int(5, 4)),
                (new EntityId(7), new Vector2Int(6, 4)),
            };

            return UniTask.FromResult(blocks);
        }
    }
}
