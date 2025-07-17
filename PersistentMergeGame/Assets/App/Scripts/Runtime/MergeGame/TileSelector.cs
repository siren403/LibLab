// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using MergeGame.Api.Inputs;
using R3;
using UnityEngine;
using VContainer.Unity;
using VitalRouter;

namespace App.MergeGame;

public readonly struct TileSelectedCommand : ICommand
{
    public GameObject Tile { get; init; }
}

public readonly struct TileDraggingCommand : ICommand
{
    public GameObject Tile { get; init; }
    public Vector3 WorldPosition { get; init; }
}

public readonly struct TileReleasedCommand : ICommand
{
    public GameObject? Target { get; init; }
    public Vector3 WorldPosition { get; init; }

    public bool HasTarget => !ReferenceEquals(Target, null);

    public bool TryGetTarget(out GameObject tile)
    {
        tile = Target!;
        return HasTarget;
    }
}

public class TileSelector : IInitializable, IDisposable
{
    private readonly InputPointerHandler _handler;
    private readonly Router _router;
    private DisposableBag _disposable;
    private Vector3 _tilePositionOffset;

    public TileSelector(InputPointerHandler handler, Router router)
    {
        _handler = handler;
        _router = router;
    }

    public void Initialize()
    {
        int hitCount = 0;
        var hits = new RaycastHit2D[1];
        int tileMask = LayerMask.GetMask("Tile");

        var input = _handler.GetObservables();

        input.onPressed.Subscribe(result =>
        {
            hitCount = Physics2D.Raycast(result.WorldPosition, Vector2.zero,
                new ContactFilter2D { useLayerMask = true, layerMask = tileMask },
                hits);
            Debug.Log($"OnPressed: hitCount={hitCount}, position={result.WorldPosition}");
            if (hitCount > 0)
            {
                var hit = hits.First();
                _tilePositionOffset = hit.transform.position - result.WorldPosition;
                _router.PublishAsync(new TileSelectedCommand { Tile = hits.First().transform.gameObject });
            }
        }).AddTo(ref _disposable);

        input.onMoved.Subscribe(result =>
        {
            if (hitCount > 0)
            {
                _router.PublishAsync(new TileDraggingCommand()
                {
                    Tile = hits.First().transform.gameObject, WorldPosition = result.WorldPosition + _tilePositionOffset
                });
            }
        }).AddTo(ref _disposable);

        input.onReleased.Subscribe(result =>
        {
            if (hitCount == 0) return;

            hitCount = Physics2D.Raycast(result.WorldPosition, Vector2.zero,
                new ContactFilter2D { useLayerMask = true, layerMask = tileMask },
                hits);
            TileReleasedCommand cmd = hitCount > 0
                ? new TileReleasedCommand { Target = hits.First().transform.gameObject, WorldPosition = result.WorldPosition }
                : new TileReleasedCommand { WorldPosition = result.WorldPosition };
            _router.PublishAsync(cmd);

            hitCount = 0;
        }).AddTo(ref _disposable);
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }
}
