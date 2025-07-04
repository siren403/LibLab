// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;

namespace MergeGame.Api.Game.MoveBlock
{
    public record MoveBlockRequest(Vector2Int FromPosition, Vector2Int ToPosition);
}
