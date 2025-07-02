// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace MergeGame.Api
{
    public class InputPointerHandler : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera = null!;


        private MouseInputProvider? _mouseProvider;
        private TouchInputProvider? _touchProvider;

        private IInputProvider Provider
        {
            get
            {
                if (TouchInputProvider.IsActivated)
                {
                    return _touchProvider ??= new TouchInputProvider(gameObject, targetCamera);
                }

                return _mouseProvider ??= new MouseInputProvider(gameObject, targetCamera);
            }
        }

        public Observable<Result> OnPressed => Provider.OnPressed;

        public Observable<Result> OnReleased => Provider.OnReleased;

        public Observable<Result> OnDragging => Provider.OnDragging;

        private void Awake()
        {
            // var hasTouch = this.UpdateAsObservable()
            //     .Where(_ => Touch.activeTouches.Any())
            //     .Select(_ => Touch.activeTouches.First());
            //
            // var onPressed = hasTouch
            //     .Select(t => (t.phase, t.screenPosition))
            //     .DistinctUntilChanged()
            //     .Where(t => t.phase == TouchPhase.Began)
            //     .Select(t => t.screenPosition);
            //
            // var onReleased = hasTouch
            //     .Select(t => (t.phase, t.screenPosition))
            //     .DistinctUntilChanged()
            //     .Where(t => t.phase == TouchPhase.Ended)
            //     .Select(t => t.screenPosition);
            //
            // var onDragging = hasTouch
            //     .Select(t => (t.phase, t.screenPosition))
            //     .DistinctUntilChanged()
            //     .Where(t => t.phase == TouchPhase.Moved)
            //     .Select(t => t.screenPosition);
            //
            //
            // onPressed.Subscribe(screen => { Debug.Log($"[Touch] OnPressed: {screen}"); }).AddTo(this);
            //
            // onReleased.Subscribe(screen => { Debug.Log($"[Touch] OnReleased: {screen}"); }).AddTo(this);
            //
            // onDragging.Subscribe(screen => { Debug.Log($"[Touch] OnDragging: {screen}"); }).AddTo(this);
        }

        private interface IInputProvider
        {
            Observable<Result> OnPressed { get; }
            Observable<Result> OnReleased { get; }
            Observable<Result> OnDragging { get; }
        }

        private class TouchInputProvider : IInputProvider
        {
            public static bool IsActivated => Touch.activeTouches.Any();

            public Observable<Result> OnPressed { get; }
            public Observable<Result> OnReleased { get; }
            public Observable<Result> OnDragging { get; }

            public TouchInputProvider(GameObject source, Camera targetCamera)
            {
                var hasTouch = source.UpdateAsObservable()
                    .Where(_ => IsActivated)
                    .Select(_ => Touch.activeTouches.First());

                OnPressed = hasTouch
                    .Select(t => (t.phase, t.screenPosition))
                    .DistinctUntilChanged()
                    .Where(t => t.phase == TouchPhase.Began)
                    .Select(t => ScreenToResult(t.screenPosition, targetCamera));

                OnReleased = hasTouch
                    .Select(t => (t.phase, t.screenPosition))
                    .DistinctUntilChanged()
                    .Where(t => t.phase == TouchPhase.Ended)
                    .Select(t => ScreenToResult(t.screenPosition, targetCamera));

                OnDragging = hasTouch
                    .Select(t => (t.phase, t.screenPosition))
                    .DistinctUntilChanged()
                    .Where(t => t.phase == TouchPhase.Moved)
                    .Select(t => ScreenToResult(t.screenPosition, targetCamera));
            }
        }

        static Result ScreenToResult(Vector2 screenPosition, Camera camera)
        {
            var worldPosition = camera.ScreenToWorldPoint(screenPosition);
            return new Result { ScreenPosition = screenPosition, WorldPosition = worldPosition };
        }

        private class MouseInputProvider : IInputProvider
        {
            public Observable<Result> OnPressed { get; }
            public Observable<Result> OnReleased { get; }
            public Observable<Result> OnDragging { get; }

            public MouseInputProvider(GameObject source, Camera targetCamera)
            {
                var hasMouse = source.UpdateAsObservable()
                    .Where(_ => Mouse.current.enabled)
                    .Select(_ => Mouse.current);

                OnPressed = hasMouse.Where(mouse => mouse.leftButton.wasPressedThisFrame)
                    .Select(mouse =>
                    {
                        var screenPosition = mouse.position.ReadValue();
                        return ScreenToResult(screenPosition, targetCamera);
                    });
                OnReleased = hasMouse.Where(mouse => mouse.leftButton.wasReleasedThisFrame)
                    .Select(mouse =>
                    {
                        var screenPosition = mouse.position.ReadValue();
                        return ScreenToResult(screenPosition, targetCamera);
                    });

                OnDragging = hasMouse.CombineLatest(
                        hasMouse.Select(mouse => mouse.leftButton),
                        (mouse, ctrl) => (mouse, ctrl))
                    .Where(input => input.ctrl.isPressed)
                    .Select(input =>
                    {
                        var screenPosition = input.mouse.position.ReadValue();
                        return ScreenToResult(screenPosition, targetCamera);
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
