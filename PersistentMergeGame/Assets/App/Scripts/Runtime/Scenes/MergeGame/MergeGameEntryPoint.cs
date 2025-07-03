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
using MergeGame.Api;
using MergeGame.Api.Extensions;
using MergeGame.Api.Game;
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
            var response = await _controller.CreateGame(new CreateGameRequest(), ct);
            if (response.Error())
            {
                throw new InvalidOperationException(
                    $"Failed to create game session: {response}");
            }

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
                new SpawnTilesCommand() { Width = response.Width, Height = response.Height },
                ct);

            #endregion

            foreach (IBoardCell cell in response.Cells)
            {
                _ = _router.PublishAsync(
                    new SpawnBlockCommand()
                    {
                        Position = new Vector2Int(cell.X, cell.Y), Id = cell.BlockId, State = cell.CellState
                    },
                    ct);
            }

            var sessionId = response.SessionId;
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
                var result = await _controller.CheckMovableCell(sessionId, cell, ctx.CancellationToken);
                if (result.ok)
                {
                    selectedCell = cell;
                    _logger.ZLogInformation(
                        $"{nameof(_controller.CheckMovableCell)}({result}, {nameof(selectedCell)}: {selectedCell.Value})");
                }
                else
                {
                    _logger.ZLogInformation(
                        $"{nameof(_controller.CheckMovableCell)}({result})");
                }
            }, CommandOrdering.Drop).AddTo(ref _disposable);

            _router.Subscribe<TileDraggingCommand>((cmd, ctx) =>
            {
                if (!selectedCell.HasValue) return;
                _focusFrame.Hide();
                _router.PublishAsync(
                    new MoveBlockPositionCommand()
                    {
                        CellPosition = selectedCell.Value, WorldPosition = cmd.WorldPosition
                    },
                    ctx.CancellationToken);
            }).AddTo(ref _disposable);

            _router.SubscribeAwait<TileReleasedCommand>(async (cmd, ctx) =>
            {
                if (!selectedCell.HasValue)
                {
                    _focusFrame.Restore();
                    return;
                }

                bool success = false;

                if (cmd.TryGetTarget(out var target)
                    && tileLookup.TryGetValue(target, out var lookup)
                    && !tileLooked.GetValueOrDefault(lookup.cell, false))
                {
                    (Vector2Int cell, Tile tile) = lookup;

                    if (cell != selectedCell.Value)
                    {
                        (bool ok,
                            IBoardCell from,
                            IBoardCell to,
                            IBoardCell spawned,
                            IReadOnlyList<IBoardCell> updatedCells) = await _controller.MergeBlock(
                            sessionId,
                            new MergeBlockRequest
                            (
                                FromPosition: selectedCell.Value,
                                ToPosition: cell
                            ), ctx.CancellationToken);
                        _logger.ZLogInformation(
                            $"{nameof(_controller.MergeBlock)}({nameof(from)}: {from}, {nameof(to)}: {to}, {nameof(spawned)}: {spawned})");
                        success = ok;

                        if (success)
                        {
                            // 합성 연출
                            _ = _router.PublishAsync(
                                new CombineBlockCommand()
                                {
                                    FromPosition = new Vector2Int(from.X, from.Y),
                                    ToPosition = new Vector2Int(to.X, to.Y),
                                }, ctx.CancellationToken);

                            _ = _router.PublishAsync(
                                new SpawnBlockCommand()
                                {
                                    Position = new Vector2Int(spawned.X, spawned.Y), Id = spawned.BlockId,
                                }, ctx.CancellationToken);

                            foreach (IBoardCell updatedCell in updatedCells)
                            {
                                _ = _router.PublishAsync(
                                    new UpdateBlockStateCommand()
                                    {
                                        Position = new Vector2Int(updatedCell.X, updatedCell.Y),
                                        State = updatedCell.CellState
                                    }, ctx.CancellationToken);
                            }
                        }
                    }
                }

                if (success)
                {
                    return;
                }

                _focusFrame.Restore();

                _ = _router.PublishAsync(new ReturnBlockPositionCommand() { Position = selectedCell.Value },
                    ctx.CancellationToken);

                _logger.ZLogTrace($"{nameof(TileReleasedCommand)}({nameof(selectedCell)}: {selectedCell})");
                selectedCell = null;
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
