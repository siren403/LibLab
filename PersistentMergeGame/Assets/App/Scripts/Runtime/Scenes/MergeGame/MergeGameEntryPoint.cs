// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using App.MergeGame;
using App.Scenes.MergeGame.Commands;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MergeGame.Api.Game;
using MergeGame.Api.Game.CreateGame;
using MergeGame.Api.Game.MoveBlock;
using MergeGame.Common.Results;
using MergeGame.Contracts.Board;
using Microsoft.Extensions.Logging;
using R3;
using UnityEngine;
using VContainer.Unity;
using VitalRouter;
using ZLogger;

namespace App.Scenes.MergeGame
{
    public class MergeGameEntryPoint : IInitializable, IAsyncStartable, IDisposable
    {
        private readonly GameController _controller;
        private readonly Router _router;
        private readonly FocusFrame _focusFrame;
        private readonly CanvasGroup _fade;
        private readonly IOnTileSpawn _onTileSpawn;
        private readonly IOnTileLock _onTileLock;
        private readonly ILogger<MergeGameEntryPoint> _logger;
        private DisposableBag _disposable;

        public MergeGameEntryPoint(
            GameController controller,
            Router router,
            FocusFrame focusFrame,
            CanvasGroup fade,
            IOnTileSpawn onTileSpawn,
            IOnTileLock onTileLock,
            ILogger<MergeGameEntryPoint> logger)
        {
            _controller = controller;
            _router = router;
            _focusFrame = focusFrame;
            _fade = fade;
            _onTileSpawn = onTileSpawn;
            _onTileLock = onTileLock;
            _logger = logger;
        }

        public void Initialize()
        {
            _fade.alpha = 1;
        }

        public async UniTask StartAsync(CancellationToken ct)
        {
            var result = await _controller.CreateGame(ct);
            if (result.IsError)
            {
                throw new InvalidOperationException($"Failed to create game session");
            }

            (Ulid sessionId, int width, int height, IBoardCell[] boardCells) = result.Value;

            #region Spawn Tiles

            var tileLookup = new Dictionary<GameObject, (Vector2Int cell, Tile tile)>();
            var tileLooked = new Dictionary<Vector2Int, bool>();

            _onTileSpawn.On.Subscribe(spawn =>
            {
                (Vector2Int cell, Tile tile) = spawn;
                tileLookup[tile.gameObject] = spawn;
            }).AddTo(ref _disposable);

            _onTileLock.On.Subscribe(lockEvent =>
            {
                (Vector2Int cell, bool isLocked) = lockEvent;
                tileLooked[cell] = isLocked;
            }).AddTo(ref _disposable);

            _ = _router.PublishAsync(
                new SpawnTilesCommand() { Width = width, Height = height },
                ct);

            #endregion

            foreach (IBoardCell cell in boardCells)
            {
                _ = _router.PublishAsync(
                    new SpawnBlockCommand()
                    {
                        Position = new Vector2Int(cell.X, cell.Y), Id = cell.BlockId, State = cell.CellState
                    },
                    ct);
            }

            Vector2Int? selectedCell = null;

            _router.SubscribeAwait<TileSelectedCommand>(async (cmd, ctx) =>
            {
                if (!tileLookup.TryGetValue(cmd.Tile, out var lookup)) return;
                (Vector2Int cell, Tile tile) = lookup;

                if (tileLooked.GetValueOrDefault(cell, false))
                {
                    return;
                }

                _focusFrame.Show(tile.transform.position);
                var movableResult = await _controller.CheckMovableCell(sessionId, cell, ctx.CancellationToken);
                if (movableResult.IsError)
                {
                    _logger.ZLogInformation(
                        $"{nameof(_controller.CheckMovableCell)}({movableResult})");
                }
                else
                {
                    // if (selectedCell.HasValue && selectedCell.Value != cell)
                    // {
                    //     await _router.PublishAsync(new TileReleasedCommand()
                    //     {
                    //         Target = tile.gameObject
                    //     }, ctx.CancellationToken);
                    // }
                    // else
                    // {
                    //     selectedCell = cell;
                    // }
                    selectedCell = cell;
                    _logger.ZLogInformation(
                        $"{nameof(_controller.CheckMovableCell)}({movableResult}, {nameof(selectedCell)}: {selectedCell.Value})");
                }
            }, CommandOrdering.Drop).AddTo(ref _disposable);

            _router.Subscribe<TileDraggingCommand>((cmd, ctx) =>
            {
                if (!selectedCell.HasValue) return;
                _focusFrame.Hide();
                _router.PublishAsync(
                    new DragBlockCommand() { CellPosition = selectedCell.Value, WorldPosition = cmd.WorldPosition },
                    ctx.CancellationToken);
            }).AddTo(ref _disposable);

            _router.SubscribeAwait<TileReleasedCommand>(async (cmd, ctx) =>
            {
                if (!selectedCell.HasValue)
                {
                    _focusFrame.Restore();
                    return;
                }

                if (!cmd.TryGetTarget(out var go))
                {
                    Fail(selectedCell.Value);
                    return;
                }

                if (!tileLookup.TryGetValue(go, out var lookup))
                {
                    Fail(selectedCell.Value);
                    return;
                }

                (Vector2Int cell, _) = lookup;

                if (tileLooked.GetValueOrDefault(cell, false))
                {
                    Fail(selectedCell.Value);
                    return;
                }

                if (cell == selectedCell.Value)
                {
                    Fail(selectedCell.Value);
                    return;
                }

                var moveBlockResult = await _controller.MoveBlock(
                    sessionId,
                    new MoveBlockRequest(selectedCell.Value, cell),
                    ctx.CancellationToken
                );

                if (moveBlockResult.IsError)
                {
                    try
                    {
                        _logger.ZLogDebug($"{nameof(_controller.MoveBlock)} failed: {moveBlockResult}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    Fail(selectedCell.Value);
                    return;
                }

                var tileSelectedCell = selectedCell.Value;

                switch (moveBlockResult.Value)
                {
                    case MovedResponse (var movedCell):
                        _ = _router.PublishAsync(
                            new MoveBlockCommand() { FromCell = tileSelectedCell, ToCell = movedCell },
                            ctx.CancellationToken);
                        break;
                    case MergedResponse(var fromCell, var mergedCell, var spawnedCell, var updatedCells):
                        _ = _router.PublishAsync(
                            new CombineBlockCommand()
                            {
                                FromPosition = new Vector2Int(fromCell.X, fromCell.Y),
                                ToPosition = new Vector2Int(mergedCell.X, mergedCell.Y),
                            }, ctx.CancellationToken);

                        _ = _router.PublishAsync(
                            new SpawnBlockCommand()
                            {
                                Position = new Vector2Int(spawnedCell.X, spawnedCell.Y), Id = spawnedCell.BlockId,
                            }, ctx.CancellationToken);

                        foreach (IBoardCell updatedCell in updatedCells)
                        {
                            _ = _router.PublishAsync(
                                new UpdateBlockStateCommand()
                                {
                                    Cell = new Vector2Int(updatedCell.X, updatedCell.Y),
                                    State = updatedCell.CellState
                                }, ctx.CancellationToken);
                        }

                        break;
                    default:
                        Fail(tileSelectedCell);
                        break;
                }

                return;

                void Fail(Vector2Int returnPosition)
                {
                    _focusFrame.Restore();

                    _ = _router.PublishAsync(new ReturnBlockPositionCommand() { Position = returnPosition },
                        ctx.CancellationToken);

                    selectedCell = null;
                    _logger.ZLogTrace($"{nameof(TileReleasedCommand)}({nameof(returnPosition)}: {returnPosition})");
                }
            }, CommandOrdering.Drop).AddTo(ref _disposable);

            await UniTask.NextFrame(ct);

            LMotion.Create(1f, 0f, 0.3f).BindToAlpha(_fade);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
