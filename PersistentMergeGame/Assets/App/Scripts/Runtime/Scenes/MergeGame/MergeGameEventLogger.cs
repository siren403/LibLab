// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using App.MergeGame;
using App.Scenes.MergeGame.Commands;
using Microsoft.Extensions.Logging;
using UnityEngine;
using VitalRouter;

namespace App.Scenes.MergeGame;

[Routes]
public partial class MergeGameEventLogger
{
    private readonly ILogger<MergeGameEventLogger> _logger;

    private Vector2Int? _movedPosition;

    public MergeGameEventLogger(ILogger<MergeGameEventLogger> logger)
    {
        _logger = logger;
    }

    [Route]
    private void On(TileSelectedCommand command)
    {
        _logger.LogTrace("Tile selected at position {Position}", command.Tile.name);
    }

    [Route]
    private void On(MoveBlockPositionCommand command)
    {
        if (_movedPosition.HasValue)
        {
            return;
        }

        _movedPosition = command.CellPosition;
        _logger.LogTrace(
            "Block moved to position {Position}", command.WorldPosition);
    }

    [Route]
    private void On(TileReleasedCommand command)
    {
        if (_movedPosition.HasValue)
        {
            _logger.LogTrace(
                "Tile dropped at position {Position}", _movedPosition.Value);
        }

        _movedPosition = null;
    }
}
