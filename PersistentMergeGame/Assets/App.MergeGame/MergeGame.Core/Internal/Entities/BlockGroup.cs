// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Entities
{
    internal record BlockGroup
    {
        public EntityId Id { get; init; }
        public string Name { get; init; } = string.Empty;

        // public override string ToString()
        // {
        //     return $"{nameof(BlockGroup)}(Id: {Id}, Name: {Name})";
        // }
    }
}
