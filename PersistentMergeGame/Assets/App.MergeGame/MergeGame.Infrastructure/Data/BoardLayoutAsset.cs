using System;
using System.Collections.Generic;
using MergeGame.Core.Enums;
using MergeGame.Core.Internal.Entities;
using MergeGame.Core.ValueObjects;
using Newtonsoft.Json;
using UnityEngine;

namespace MergeGame.Infrastructure.Data
{
    [CreateAssetMenu(fileName = "BoardLayout", menuName = "Merge Game/Board Layout", order = 0)]
    public class BoardLayoutAsset : ScriptableObject
    {
        [SerializeField] private TextAsset json = null!;
        [SerializeField] private int scalePerUnit = 10;

        public BoardLayout ToBoardLayout(EntityId id)
        {
            Data? data = JsonConvert.DeserializeObject<Data>(json.text);
            if (data == null)
            {
                throw new InvalidOperationException("Failed to deserialize BoardLayoutAsset data.");
            }

            int width = data.Width / scalePerUnit;
            int height = data.Height / scalePerUnit;
            List<BoardCellSpec> cells = new();

            foreach (var entityGroup in data.Entities)
            {
                foreach (var entity in entityGroup.Value)
                {
                    int x = entity.X / scalePerUnit;
                    int y = entity.Y / scalePerUnit;
                    y = height - 1 - y;

                    var customFields = entity.CustomFields;
                    BlockId blockId = customFields.Block switch
                    {
                        "block_bag_1" => 0,
                        "block_bag_2" => 1,
                        "block_bag_3" => 2,

                        "block_bread_1" => 3,
                        "block_bread_2" => 4,
                        "block_bread_3" => 5,
                        "block_bread_4" => 6,
                        "block_bread_5" => 7,

                        "undefined" => 10,

                        "block_untouchable_1" => 10000,
                        "block_untouchable_2" => 10001,
                        "block_untouchable_3" => 10002,
                        _ => throw new InvalidOperationException($"Unknown block type: {customFields.Block}")
                    };
                    BoardCellState cellState = customFields.BlockState switch
                    {
                        "untouchable" => BoardCellState.Untouchable,
                        "mergeable" => BoardCellState.Mergeable,
                        "movable" => BoardCellState.Movable,
                        _ => throw new InvalidOperationException($"Unknown place type: {customFields.BlockState}")
                    };

                    var cellSpec = new BoardCellSpec(
                        new Position(x, y),
                        blockId,
                        cellState
                    );

                    cells.Add(cellSpec);
                }
            }

            return new BoardLayout(id, width, height, cells.ToArray());
        }

        [Serializable]
        public record Data
        {
            [JsonProperty("width")] public int Width { get; set; }
            [JsonProperty("height")] public int Height { get; set; }
            [JsonProperty("entities")] public Dictionary<string, Entity[]> Entities { get; set; } = null!;
        }

        [Serializable]
        public record Entity
        {
            [JsonProperty("x")] public int X { get; set; }
            [JsonProperty("y")] public int Y { get; set; }
            [JsonProperty("customFields")] public CustomFields CustomFields { get; set; } = new();
        }

        [Serializable]
        public record CustomFields
        {
            [JsonProperty("block")] public string Block { get; set; } = string.Empty;
            [JsonProperty("block_state")] public string BlockState { get; set; } = string.Empty;
        }
    }
}
