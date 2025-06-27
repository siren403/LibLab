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
        private readonly IMediator _mediator;
        private readonly Router _router;
        private readonly ILogger<MergeGameEntryPoint> _logger;
        private DisposableBag _disposable;

        public MergeGameEntryPoint(
            GameController controller,
            IMediator mediator,
            Router router,
            ILogger<MergeGameEntryPoint> logger)
        {
            _controller = controller;
            _mediator = mediator;
            _router = router;
            _logger = logger;
        }

        public void Initialize()
        {
            _router.Subscribe<SelectBlockCommand>((cmd, ctx) =>
            {
                _logger.LogInformation("Block selected: position {Position}", cmd.Position);
            }).AddTo(ref _disposable);
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
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
