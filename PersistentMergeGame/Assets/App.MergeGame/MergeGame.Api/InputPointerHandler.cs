// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MergeGame.Api
{
    public class InputPointerHandler : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera = null!;

        private Observable<Mouse>? _hasMouse;

        private Observable<Mouse> HasMouse =>
            _hasMouse ??= this.UpdateAsObservable()
                .Where(_ => Mouse.current.enabled)
                .Select(_ => Mouse.current);

        public Observable<Result> OnPressed
        {
            get
            {
                return HasMouse.Where(mouse => mouse.leftButton.wasPressedThisFrame)
                    .Select(mouse =>
                    {
                        var screenPosition = mouse.position.ReadValue();
                        var worldPosition = targetCamera.ScreenToWorldPoint(screenPosition);
                        return new Result { ScreenPosition = screenPosition, WorldPosition = worldPosition };
                    });
            }
        }

        public Observable<Result> OnReleased
        {
            get
            {
                return HasMouse.Where(mouse => mouse.leftButton.wasReleasedThisFrame)
                    .Select(mouse =>
                    {
                        var screenPosition = mouse.position.ReadValue();
                        var worldPosition = targetCamera.ScreenToWorldPoint(screenPosition);
                        return new Result { ScreenPosition = screenPosition, WorldPosition = worldPosition };
                    });
            }
        }

        public Observable<Result> OnDragging
        {
            get
            {
                return HasMouse.CombineLatest(
                        HasMouse.Select(mouse => mouse.leftButton),
                        (mouse, ctrl) => (mouse, ctrl))
                    .Where(input => input.ctrl.isPressed)
                    .Select(input =>
                    {
                        var screenPosition = input.mouse.position.ReadValue();
                        var worldPosition = targetCamera.ScreenToWorldPoint(screenPosition);
                        return new Result { ScreenPosition = screenPosition, WorldPosition = worldPosition };
                    })
                    .DistinctUntilChanged();
            }
        }

        public readonly struct Result
        {
            public readonly Vector2 ScreenPosition { get; init; }
            public readonly Vector3 WorldPosition { get; init; }
        }
    }
}
