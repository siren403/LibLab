// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using App.MergeGame.Core.Enums;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Application.Data
{
    public record BoardCell(
        Position Position,
        BlockId BlockId,
        PlaceBlockType PlaceType
    );
}
