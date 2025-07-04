// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Extensions;
using MergeGame.Core.ValueObjects;
using UnityEngine;

namespace MergeGame.Api.Game
{
    public partial class GameController
    {
        public async UniTask<(bool ok, long blockId)> CheckMovableCell(
            Ulid sessionId,
            Vector2Int position,
            CancellationToken ct = default
        )
        {
            try
            {
                var result = await _mediator.ExecuteCheckMovableCell(
                    new CheckMovableCellCommand()
                    {
                        SessionId = sessionId, Position = new Position(position.x, position.y)
                    },
                    ct);

                return (result.IsMovable, result.BlockId);
            }
            catch
            {
                return (false, BlockId.Invalid);
            }
        }
    }
}
