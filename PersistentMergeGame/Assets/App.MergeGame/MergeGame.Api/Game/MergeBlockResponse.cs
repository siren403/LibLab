// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using MergeGame.Contracts.Board;

namespace MergeGame.Api.Game
{
    public record MergeBlockResponse(
        bool Ok,
        IBoardCell FromCell,
        IBoardCell ToCell,
        IBoardCell SpawnedCell,
        IReadOnlyList<IBoardCell> UpdatedCells
    )
    {
        public static MergeBlockResponse Error = new(false, null!, null!, null!, ArraySegment<IBoardCell>.Empty);
    }
}
