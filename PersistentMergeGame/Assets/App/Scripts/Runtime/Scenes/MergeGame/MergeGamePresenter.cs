using System.Collections.Generic;
using System.Linq;
using App.MergeGame;
using App.MergeGame.Data;
using App.Scenes.MergeGame.Commands;
using Com.LuisPedroFonseca.ProCamera2D;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VitalRouter;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace App.Scenes.MergeGame
{
    public interface IOnTileSpawn
    {
        Observable<(Vector2Int cell, Tile tile)> On { get; }
    }

    public interface IOnTileLock
    {
        Observable<(Vector2Int cell, bool isLocked)> On { get; }
    }

    [Routes]
    public partial class MergeGamePresenter : MonoBehaviour, IOnTileSpawn, IOnTileLock
    {
        [SerializeField] private Tile tilePrefab = null!;
        [SerializeField] private List<BlockData> blockData = new();
        [SerializeField] private BlockData fallbackBlockData = null!;
        [SerializeField] private Vector2 origin = new(0.5f, 0.5f);
        [SerializeField] private ProCamera2DContentFitter cameraContentFitter = null!;
        [SerializeField] private float contentMargin = 0.1f;

        private TilePositionCalculator _positionCalculator;

        private readonly Dictionary<Vector2Int, Block> _blocks = new();
        private readonly Dictionary<Vector2Int, Tile> _tiles = new();

        private Dictionary<long, BlockData>? _blockData;

        #region Events

        private readonly Subject<(Vector2Int cell, Tile tile)> _onTileSpawn = new();
        private readonly Subject<(Vector2Int cell, bool isLocked)> _onTileLock = new();

        Observable<(Vector2Int cell, Tile tile)> IOnTileSpawn.On => _onTileSpawn;
        Observable<(Vector2Int cell, bool isLocked)> IOnTileLock.On => _onTileLock;

        #endregion

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
            if (_combineMotionHandles.TryGetValue(command.Position, out var handle))
            {
                _ = UniTask.WaitWhile(handle,
                    state => state.IsPlaying(),
                    cancellationToken: destroyCancellationToken
                ).ContinueWith(InstantiateBlock);
            }
            else
            {
                InstantiateBlock();
            }

            return;

            void InstantiateBlock()
            {
                _combineMotionHandles.Remove(command.Position);
                var position = _positionCalculator.GetTilePosition(command.Position);
                if (!(_blockData?.TryGetValue(command.Id, out BlockData data) ?? false))
                {
                    data = fallbackBlockData;
                }

                Block block = Instantiate(
                    data.Prefab,
                    position,
                    Quaternion.identity,
                    transform
                );
                block.State = command.State;
                _blocks[command.Position] = block;

                LMotion.Create(Vector3.zero, Vector3.one, 0.15f)
                    .WithEase(Ease.OutBack)
                    .BindToLocalScale(block.transform)
                    .AddTo(this);
            }
        }

        [Route]
        private void On(DragBlockCommand command)
        {
            if (!_blocks.TryGetValue(command.CellPosition, out var block))
            {
                return;
            }

            block.transform.position = command.WorldPosition;
            block.OnDragPosition();
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
            _tiles[position] = tile;
            tile.transform.position = calculator.GetTilePosition(position);
            tile.OnChangedPosition(position);
            _onTileSpawn.OnNext((position, tile));
        }

        private readonly Dictionary<Vector2Int, MotionHandle> _combineMotionHandles = new();

        [Route]
        private void On(CombineBlockCommand command)
        {
            var dir = (Vector2)command.ToPosition - command.FromPosition;
            dir.Normalize();

            DestroyBlock(command.FromPosition, command.ToPosition, dir);
            DestroyBlock(command.ToPosition, command.ToPosition, -dir);

            return;

            void DestroyBlock(Vector2Int cell, Vector2Int spawned, Vector2 direction, float startDistance = 1f)
            {
                if (!_blocks.Remove(cell, out var block))
                {
                    return;
                }

                if (!_tiles.TryGetValue(spawned, out var tile))
                {
                    Destroy(block.gameObject);
                    return;
                }

                block.OnDragPosition();
                block.Color = Color.white;

                var to = tile.transform.position;
                var from = to + (Vector3)direction * startDistance;

                _onTileLock.OnNext((cell, true));
                var handle = LSequence.Create()
                    .Append(
                        LMotion.Create(block.transform.position, from, 0.2f)
                            .WithEase(Ease.OutQuint)
                            .BindToPosition(block.transform)
                    )
                    .Append(
                        LMotion.Create(from, to, 0.2f)
                            .WithEase(Ease.OutQuint)
                            .BindToPosition(block.transform)
                    )
                    .Run();

                _ = UniTask.WaitWhile(handle, static state => state.IsPlaying(),
                        cancellationToken: destroyCancellationToken)
                    .ContinueWith(() =>
                    {
                        Destroy(block.gameObject);
                        _onTileLock.OnNext((cell, false));
                    });

                _combineMotionHandles[spawned] = handle.AddTo(this);
            }
        }

        [Route]
        private void On(UpdateBlockStateCommand command)
        {
            if (!_blocks.TryGetValue(command.Cell, out var block))
            {
                return;
            }

            block.State = command.State;
        }

        [Route]
        private void On(MoveBlockCommand command)
        {
            if (!_blocks.TryGetValue(command.FromCell, out var block))
            {
                return;
            }

            if (!_tiles.TryGetValue(command.ToCell, out var toTile))
            {
                return;
            }

            _blocks.Remove(command.FromCell);
            _blocks[command.ToCell] = block;
            block.OnReturnPosition();

            var toPosition = toTile.transform.position;
            _ = LMotion.Create(block.transform.position, toPosition, 0.2f)
                .WithEase(Ease.OutQuint)
                .BindToPosition(block.transform)
                .AddTo(this);
        }
    }
}
