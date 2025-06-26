// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Entities
{
    internal record BlockSpawner
    {
        public EntityId Id { get; init; }
        public EntityId FromBlockId { get; init; }
        public EntityId[] SpawnBlockIds { get; init; } = Array.Empty<EntityId>();

        // public override string ToString()
        // {
        //     return
        //         $"{nameof(BlockSpawner)}(Id: {Id}, {nameof(FromBlockId)}: {FromBlockId}, {nameof(SpawnBlockIds)}: [{string.Join(", ", SpawnBlockIds)}])";
        // }
    }
}
