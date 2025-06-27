using System.Collections.Generic;
using System.Linq;
using App.MergeGame;
using App.MergeGame.Data;
using App.Scenes.MergeGame.Commands;
using Com.LuisPedroFonseca.ProCamera2D;
using LitMotion;
using LitMotion.Extensions;
using R3;
using UnityEngine;
using VContainer;
using VitalRouter;

namespace App.Scenes.MergeGame
{
    [Routes]
    public partial class MergeGamePresenter : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera = null!;
        [SerializeField] private Tile tilePrefab = null!;
        [SerializeField] private List<BlockData> blockData = new();
        [SerializeField] private Vector2 origin = new(0.5f, 0.5f);
        [SerializeField] private ProCamera2DContentFitter cameraContentFitter = null!;
        [SerializeField] private float contentMargin = 0.1f;

        private TilePositionCalculator _positionCalculator;

        private readonly Dictionary<Vector2Int, Block> _blocks = new();
        private readonly Dictionary<Vector2Int, Tile> _tiles = new();

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
            _blocks[command.Position] = block;
        }

        [Route]
        private void On(MoveBlockPositionCommand positionCommand)
        {
            if (!_blocks.TryGetValue(positionCommand.Position, out var block))
            {
                return;
            }

            var inputPosition = targetCamera.ScreenToWorldPoint(Input.mousePosition);
            block.transform.position = inputPosition;
            block.OnMovePosition();
        }

        private MotionHandle _returnBlockHandle = MotionHandle.None;

        [Route]
        private void On(ReturnBlockPositionCommand command)
        {
            if (!_tiles.TryGetValue(command.Position, out var tile))
            {
                return;
            }

            if (!_blocks.TryGetValue(command.Position, out var block))
            {
                return;
            }

            var fromPosition = block.transform.position;
            var toPosition = tile.transform.position;

            _returnBlockHandle.TryComplete();
            _returnBlockHandle = LMotion.Create(fromPosition, toPosition, 0.05f)
                .WithOnComplete(() => { block.OnReturnPosition(); })
                .BindToPosition(block.transform);
        }

        private void SpawnTile(int x, int y, TilePositionCalculator calculator)
        {
            var position = new Vector2Int(x, y);
            Tile tile = Instantiate(tilePrefab, transform)!;
            tile.name = $"Tile {x} {y}";

            tile.OnPressed.Subscribe(selected =>
            {
                _router?.PublishAsync(new SelectTileCommand
                {
                    CellPosition = position, WorldPosition = selected.transform.position
                });
            }).AddTo(this);

            tile.OnDragged.Subscribe(dragged =>
            {
                _router?.PublishAsync(new DragTileCommand() { CellPosition = position });
            }).AddTo(this);

            tile.OnReleased.Subscribe(released =>
            {
                _router?.PublishAsync(new DropTileCommand() { CellPosition = position });
            }).AddTo(this);

            _tiles[position] = tile;

            tile.transform.position = calculator.GetTilePosition(position);

            tile.OnChangedPosition(position);
        }
    }
}
