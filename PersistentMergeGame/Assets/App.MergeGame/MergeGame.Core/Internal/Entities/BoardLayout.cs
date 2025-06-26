// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Entities
{
    public record BoardLayout(EntityId Id, int Width, int Height, BoardCellSpec[] Cells);
}
