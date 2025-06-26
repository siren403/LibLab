// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MergeGame.Core.Internal.Repositories;
using MergeGame.Core.ValueObjects;
using MergeGame.Infrastructure.Models;
using Supabase;
using Entities = MergeGame.Core.Internal.Entities;

namespace MergeGame.Infrastructure.Repositories
{
    internal class BlockRepository : IBlockRepository
    {
        private readonly Client _client;
        private readonly Dictionary<EntityId, Entities.Block> _blocks = new(8);

        public BlockRepository(Client client)
        {
            _client = client;
        }

        public async ValueTask<Entities.Block> GetBlock(EntityId id, CancellationToken cancellationToken = default)
        {
            if (_blocks.TryGetValue(id, out var cached))
            {
                return cached;
            }

            var resp = await _client.From<Models.Block>()
                .Where(x => x.Id == id)
                .Single(cancellationToken);

            var block = resp!.ToEntity();
            _blocks[id] = block;
            return block;
        }
    }
}
