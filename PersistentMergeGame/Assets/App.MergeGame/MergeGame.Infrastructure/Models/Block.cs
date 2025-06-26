// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MergeGame.Infrastructure.Models
{
    using Entities = Core.Internal.Entities;
    using Enums = Core.Enums;

    [Table("block")]
    public class Block : BaseModel
    {
        [PrimaryKey("id")] public long Id { get; init; }

        [Column("name")] public string Name { get; init; }

        [Column("block_type")] public short BlockType { get; init; }

        [Column("block_group")] public long BlockGroup { get; init; }

        [Column("created_at")] public DateTime CreatedAt { get; init; }

        public override string ToString()
        {
            return
                $"{nameof(Block)}(Id: {Id}, Name: {Name}, {nameof(BlockType)}: {BlockType}, {nameof(BlockGroup)}: {BlockGroup}, CreatedAt: {CreatedAt})";
        }
    }

    public static class BlockExtensions
    {
        internal static Entities.Block ToEntity(this Block model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            return new Entities.Block()
            {
                Id = model.Id,
                BlockType = model.BlockType switch
                {
                    1 => Enums.BlockType.None,
                    2 => Enums.BlockType.Spawner,
                    3 => Enums.BlockType.Currency,
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(model.BlockType),
                        model.BlockType,
                        $"Unknown {nameof(model.BlockType)}"
                    )
                },
                BlockGroupId = model.BlockGroup
            };
        }
    }
}
