using System.Collections.Generic;
using System.Linq;
using App.MergeGame;
using App.MergeGame.Data;
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
        [SerializeField] private List<BlockData> blockData = new();
        [SerializeField] private Vector2 origin = new(0.5f, 0.5f);
        [SerializeField] private ProCamera2DContentFitter cameraContentFitter = null!;
        [SerializeField] private float contentMargin = 0.1f;

        private TilePositionCalculator _positionCalculator;

        private readonly Dictionary<Block, Vector2Int> _blockPositions = new();

        [Inject] private Router? _router;

        private Dictionary<long, BlockData>? _blockData;

        private void Awake()
        {
            _blockData = blockData.ToDictionary(data => data.Id, data => data);
        }

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
            var position = _positionCalculator.GetTilePosition(command.Position);
            if (!(_blockData?.TryGetValue(command.Id, out BlockData data) ?? false))
            {
                return;
            }

            Block block = Instantiate(
                data.Prefab,
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
