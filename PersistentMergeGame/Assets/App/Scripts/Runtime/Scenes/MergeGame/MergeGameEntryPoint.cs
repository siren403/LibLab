// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using App.Scenes.MergeGame.Commands;
using Cysharp.Threading.Tasks;
using MergeGame.Api;
using MergeGame.Api.Extensions;
using MergeGame.Api.Game;
using MergeGame.Contracts.Board;
using Microsoft.Extensions.Logging;
using R3;
using UnityEngine;
using VContainer.Unity;
using VExtensions.Mediator.Abstractions;
using VitalRouter;

namespace App.Scenes.MergeGame
{
    public class MergeGameEntryPoint : IInitializable, IAsyncStartable, IDisposable
    {
        private readonly GameController _controller;
        private readonly Router _router;
        private readonly FocusFrame _focusFrame;
        private readonly ILogger<MergeGameEntryPoint> _logger;
        private DisposableBag _disposable;

        public MergeGameEntryPoint(
            GameController controller,
            Router router,
            FocusFrame focusFrame,
            ILogger<MergeGameEntryPoint> logger)
        {
            _controller = controller;
            _router = router;
            _focusFrame = focusFrame;
            _logger = logger;
        }

        public void Initialize()
        {
        }

        public async UniTask StartAsync(CancellationToken ct)
        {
            var response = await _controller.CreateGame(new CreateGameRequest(), ct);
            if (response.Error())
            {
                throw new InvalidOperationException(
                    $"Failed to create game session: {response}");
            }

            _ = _router.PublishAsync(
                new SpawnTilesCommand() { Width = response.Width, Height = response.Height },
                ct);

            foreach (IBoardCell cell in response.Cells)
            {
                _ = _router.PublishAsync(
                    new SpawnBlockCommand() { Position = new Vector2Int(cell.X, cell.Y), Id = cell.BlockId, },
                    ct);
            }

            var sessionId = response.SessionId;
            bool isMovable = false;
            bool isMoved = false;
            _router.SubscribeAwait<SelectTileCommand>(async (cmd, ctx) =>
            {
                _focusFrame.Show(cmd.WorldPosition);
                isMovable = await _controller.IsMovableCell(sessionId, cmd.CellPosition, ctx.CancellationToken);
            }, CommandOrdering.Drop).AddTo(ref _disposable);
            _router.Subscribe<DragTileCommand>((cmd, ctx) =>
            {
                if (!isMovable) return;
                isMoved = true;
                _focusFrame.Hide();
                _router.PublishAsync(new MoveBlockPositionCommand() { Position = cmd.CellPosition },
                    ctx.CancellationToken);
            }).AddTo(ref _disposable);

            _router.Subscribe<DropTileCommand>((cmd, ctx) =>
            {
                if (isMoved)
                {
                    _focusFrame.Restore();
                }

                isMovable = false;
                isMoved = false;
                _router.PublishAsync(new ReturnBlockPositionCommand() { Position = cmd.CellPosition },
                    ctx.CancellationToken);
            }).AddTo(ref _disposable);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
