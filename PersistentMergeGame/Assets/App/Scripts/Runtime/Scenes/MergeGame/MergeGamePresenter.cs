// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using App.Scenes.MergeGame.Commands;
using Com.LuisPedroFonseca.ProCamera2D;
using R3;
using UnityEngine;
using VContainer;
using VitalRouter;

namespace App.Scenes.MergeGame
{
    [Routes]
    public partial class MergeGamePresenter : MonoBehaviour
    {
        [SerializeField] private Tile tilePrefab = null!;
        [SerializeField] private List<Block> stuffPrefabs = new();
        [SerializeField] private Vector2 origin = new Vector2(0.5f, 0.5f);
        [SerializeField] private ProCamera2DContentFitter cameraContentFitter = null!;
        [SerializeField] private float contentMargin = 0.1f;

        private TilePositionCalculator? _positionCalculator;

        private readonly Dictionary<Block, Vector2Int> _blockPositions = new();

        [Inject] private Router? _router;

        [Route]
        private void On(SpawnTilesCommand command)
        {
            var tileSize = tilePrefab.Size;
            var boardSize = new Vector2(
                command.Width * tileSize.x,
                command.Height * tileSize.y
            );

            cameraContentFitter.TargetWidth = boardSize.x + contentMargin;

            _positionCalculator = new TilePositionCalculator(
                tileSize,
                boardSize,
                origin
            );

            for (int y = 0; y < command.Height; y++)
            {
                for (int x = 0; x < command.Width; x++)
                {
                    SpawnTile(x, y, _positionCalculator);
                }
            }
        }

        [Route]
        private void On(SpawnBlockCommand command)
        {
            if (_positionCalculator == null)
            {
                throw new System.InvalidOperationException(
                    "Position calculator is not initialized. Ensure that tiles are spawned first."
                );
            }

            var position = _positionCalculator.GetTilePosition(command.Position);
            int index = (int)command.Id - 1;
            Block block = Instantiate(
                stuffPrefabs[index],
                position,
                Quaternion.identity,
                transform
            );
            _blockPositions[block] = command.Position;
            block.OnClicked.Subscribe(b =>
            {
                if (_blockPositions.TryGetValue(b, out var pos))
                {
                    _router?.PublishAsync(new SelectBlockCommand() { Position = pos });
                }
            }).AddTo(this);
        }

        private void SpawnTile(int x, int y, TilePositionCalculator calculator)
        {
            Tile tile = Instantiate(tilePrefab, transform)!;
            tile.name = $"Tile {x} {y}";

            var position = new Vector2Int(x, y);

            tile.transform.position = calculator.GetTilePosition(position);
            tile.OnChangedPosition(position);
        }
    }
}
