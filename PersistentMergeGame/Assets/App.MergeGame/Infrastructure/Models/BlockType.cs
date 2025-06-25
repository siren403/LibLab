// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MergeGame.Infrastructure.Models
{
    [Table("block_type")]
    public class BlockType : BaseModel
    {
        [PrimaryKey("id")] public short Id { get; init; }
        [Column("name")] public string Name { get; init; } = string.Empty;

        public override string ToString()
        {
            return $"{nameof(BlockType)}(Id: {Id}, Name: {Name})";
        }
    }
}
