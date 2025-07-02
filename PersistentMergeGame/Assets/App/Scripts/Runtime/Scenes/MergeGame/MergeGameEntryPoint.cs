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
        private readonly IOnSpawnTile _onSpawnTile;
        private readonly ILogger<MergeGameEntryPoint> _logger;
        private DisposableBag _disposable;

        public MergeGameEntryPoint(
            GameController controller,
            Router router,
            FocusFrame focusFrame,
            CanvasGroup fade,
            IOnSpawnTile onSpawnTile,
            ILogger<MergeGameEntryPoint> logger)
        {
            _controller = controller;
            _router = router;
            _focusFrame = focusFrame;
            _fade = fade;
            _onSpawnTile = onSpawnTile;
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

            var tiles = new Dictionary<GameObject, (Vector2Int cell, Tile tile)>();

            // TODO: to ICommandHandler<(pos, tile)[]>
            _onSpawnTile.On.Subscribe(spawn =>
            {
                (Vector2Int cell, Tile tile) = spawn;
                tiles[tile.gameObject] = spawn;
            }).AddTo(ref _disposable);

            _ = _router.PublishAsync(
                new SpawnTilesCommand() { Width = response.Width, Height = response.Height },
                ct);

            #endregion

            foreach (IBoardCell cell in response.Cells)
            {
                _ = _router.PublishAsync(
                    new SpawnBlockCommand() { Position = new Vector2Int(cell.X, cell.Y), Id = cell.BlockId, },
                    ct);
            }

            var sessionId = response.SessionId;
            Vector2Int? selectedCell = null;

            _router.SubscribeAwait<TileSelectedCommand>(async (cmd, ctx) =>
            {
                if (!tiles.TryGetValue(cmd.Tile, out var lookup)) return;
                (Vector2Int cell, Tile tile) = lookup;

                _focusFrame.Show(tile.transform.position);
                bool isMovable = await _controller.IsMovableCell(sessionId, cell, ctx.CancellationToken);
                if (isMovable)
                {
                    selectedCell = cell;
                    _logger.ZLogTrace(
                        $"{nameof(TileSelectedCommand)}({nameof(isMovable)}: {isMovable}, {nameof(selectedCell)}: {selectedCell.Value})");
                }
                else
                {
                    _logger.ZLogTrace(
                        $"{nameof(TileSelectedCommand)}({nameof(isMovable)}: {isMovable})");
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

            _router.Subscribe<TileReleasedCommand>((cmd, ctx) =>
                {
                    _focusFrame.Restore();

                    if (!selectedCell.HasValue) return;

                    bool mergeSuccess = false;

                    if (cmd.TryGetTarget(out var target) && tiles.TryGetValue(target, out var lookup))
                    {
                        (Vector2Int cell, Tile tile) = lookup;
                        if (cell != selectedCell.Value)
                        {
                            // TODO: try to merge
                            _logger.ZLogCritical($"TODO: Try to merge {selectedCell.Value} with {cell}");
                        }
                    }

                    if (mergeSuccess)
                    {
                        return;
                    }

                    _router.PublishAsync(new ReturnBlockPositionCommand() { Position = selectedCell.Value },
                        ctx.CancellationToken);

                    _logger.ZLogTrace($"{nameof(TileReleasedCommand)}({nameof(selectedCell)}: {selectedCell})");
                    selectedCell = null;
                })
                .AddTo(ref _disposable);

            await UniTask.NextFrame(ct);

            LMotion.Create(1f, 0f, 0.3f).BindToAlpha(_fade);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
