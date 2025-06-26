// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using MergeGame.Core.Internal.Entities;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Repositories
{
    internal interface IBlockRepository
    {
        ValueTask<Block> GetBlock(EntityId id, CancellationToken cancellationToken = default);
    }
}
