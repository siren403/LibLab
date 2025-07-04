// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Common;
using UnityEngine;

namespace MergeGame.Api.Game.MoveBlock
{
    public record MoveBlockResult(int StatusCode) : Result(StatusCode)
    {
        public static MoveBlockError Error(string message) => new MoveBlockError(message);
    }

    public record MoveBlockError(string Message) : MoveBlockResult(-1);

    public record MovedResult(Vector2Int ToCell) : MoveBlockResult(0);

    public record MergedResult(MergedResponse Response) : MoveBlockResult(0);
}
