// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using MergeGame.Contracts.Board;

namespace MergeGame.Api.Game.MoveBlock
{
    public record MergedResponse(
        IBoardCell FromCell,
        IBoardCell ToCell,
        IBoardCell SpawnedCell,
        IReadOnlyList<IBoardCell> UpdatedCells
    );
}
