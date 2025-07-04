// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MergeGame.Infrastructure.Models
{
    [Table("block_merge_rule")]
    public class BlockMergeRule : BaseModel
    {
        [PrimaryKey("id")] public long Id { get; init; }
        [PrimaryKey("source_block_id")] public long SourceBlockId { get; init; }
        [Column("next_block_id")] public long NextBlockId { get; init; }
        [Column("created_at")] public DateTime CreatedAt { get; init; }
    }
}
