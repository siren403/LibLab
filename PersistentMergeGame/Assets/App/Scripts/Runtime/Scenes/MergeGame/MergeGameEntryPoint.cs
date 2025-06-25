// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using App.Scenes.MergeGame.Commands;
using Cysharp.Threading.Tasks;
using MergeGame.Api;
using MergeGame.Core.ValueObjects;
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
        private readonly MergeGameController _controller;
        private readonly IMediator _mediator;
        private readonly Router _router;
        private readonly ILogger<MergeGameEntryPoint> _logger;
        private DisposableBag _disposable;

        public MergeGameEntryPoint(
            MergeGameController controller,
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
            (int width, int height) = await _controller.CreateBoard(ct);
            await _router.PublishAsync(new SpawnTilesCommand() { Width = width, Height = height }, ct);

            var blocks = await _controller.GetBlocks(ct);
            foreach ((EntityId, Vector2Int) block in blocks)
            {
                (EntityId id, Vector2Int position) = block;
                _ = _router.PublishAsync(new SpawnBlockCommand() { Id = id, Position = position }, ct);
            }
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
