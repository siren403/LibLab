// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.Enums;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Entities
{
    internal record Block
    {
        public BlockId Id { get; init; }
        public BlockType BlockType { get; init; }
        public EntityId BlockGroupId { get; init; }

        // public override string ToString()
        // {
        //     return $"{nameof(Block)}(Id: {Id}, {nameof(BlockType)}: {BlockType}, {nameof(BlockGroupId)}: {BlockGroupId})";
        // }
    }
}
