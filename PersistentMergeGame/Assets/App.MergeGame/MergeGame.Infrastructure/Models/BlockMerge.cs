// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MergeGame.Infrastructure.Models
{
    [Table("block_merge")]
    public class BlockMerge : BaseModel
    {
        [PrimaryKey("id")] public long Id { get; init; }
        [PrimaryKey("from_block_id")] public long FromBlockId { get; init; }
        [Column("to_block_id")] public long ToBlockId { get; init; }
        [Column("created_at")] public DateTime CreatedAt { get; init; }

        public override string ToString()
        {
            return
                $"{nameof(BlockMerge)}(Id: {Id}, {nameof(FromBlockId)}: {FromBlockId}, {nameof(ToBlockId)}: {ToBlockId}, CreatedAt: {CreatedAt})";
        }
    }
}
