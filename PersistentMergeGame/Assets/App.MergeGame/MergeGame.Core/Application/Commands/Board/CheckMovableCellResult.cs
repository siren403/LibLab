// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Application.Commands.Board
{
    public readonly struct CheckMovableCellResult
    {
        public bool IsMovable { get; init; }
        public BlockId BlockId { get; init; }
    }
}
