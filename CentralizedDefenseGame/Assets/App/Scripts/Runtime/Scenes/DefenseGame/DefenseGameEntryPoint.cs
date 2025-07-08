// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefenseGame.Api.Game;
using Microsoft.Extensions.Logging;
using VContainer.Unity;
using ZLogger;

namespace App.Scenes.DefenseGame
{
    public class DefenseGameEntryPoint : IInitializable, IAsyncStartable
    {
        private readonly GameController _controller;
        private readonly DefenseGamePresenter _presenter;
        private readonly ILogger<DefenseGameEntryPoint> _logger;

        public DefenseGameEntryPoint(
            GameController controller,
            DefenseGamePresenter presenter,
            ILogger<DefenseGameEntryPoint> logger)
        {
            _controller = controller;
            _presenter = presenter;
            _logger = logger;
        }

        public void Initialize()
        {
        }

        public async UniTask StartAsync(CancellationToken ct = default)
        {
            var result = await _controller.CreateGame(ct);
            if (result.IsError)
            {
                _logger.ZLogError($"Failed to create game: {result.Errors}");
                return;
            }

            (Ulid sessionId, float radius) = result.Value;
            _presenter.SetRadius(radius);


        }
    }
}
