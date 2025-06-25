// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.Enums;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Entities
{
    internal class Block
    {
        public EntityId Id { get; init; }
        public BlockType BlockType { get; init; }
        public EntityId BlockGroupId { get; init; }

        public override string ToString()
        {
            return $"Stuff(Id: {Id}, {nameof(BlockType)}: {BlockType}, {nameof(BlockGroupId)}: {BlockGroupId})";
        }
    }
}
