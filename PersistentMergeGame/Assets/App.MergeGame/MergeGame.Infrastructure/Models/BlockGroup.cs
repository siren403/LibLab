// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MergeGame.Infrastructure.Models
{
    [Table("block_group")]
    public class BlockGroup : BaseModel
    {
        [PrimaryKey("id")] public long Id { get; init; }

        [Column("name")] public string Name { get; init; }

        [Column("created_at")] public DateTime CreatedAt { get; init; }

        public override string ToString()
        {
            return $"{nameof(BlockGroup)}(Id: {Id}, Name: {Name}, CreatedAt: {CreatedAt})";
        }
    }
}
