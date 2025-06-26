// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Entities
{
    internal record BlockMerge
    {
        public EntityId Id { get; init; }
        public BlockMergeLevel Level { get; init; }
        public EntityId FromBlockId { get; init; }
        public EntityId ToBlockId { get; init; }

        // public override string ToString()
        // {
        //     return
        //         $"{nameof(BlockMerge)}(Id: {Id}, Level: {Level}, {nameof(FromBlockId)}: {FromBlockId}, {nameof(ToBlockId)}: {ToBlockId})";
        // }
    }
}
