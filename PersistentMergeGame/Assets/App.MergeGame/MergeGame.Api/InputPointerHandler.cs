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

        // WebGL(Mobile)에서는 EnhancedTouch 동작하지 않음.
        // private EnhancedTouchProvider? _enhancedTouchProvider;

        // WebGL(PC)
        private MouseInputProvider? _mouseProvider;

        // WebGL(Mobile)
        private InputTouchesProvider? _inputTouchesProvider;

        private readonly Subject<Result> _onPressed = new();
        private readonly Subject<Result> _onReleased = new();
        private readonly Subject<Result> _onMoved = new();

        public (Observable<Result> onPressed, Observable<Result> onReleased, Observable<Result> onMoved)
            GetObservables()
        {
            _mouseProvider ??= new MouseInputProvider(this).AddTo(this);
            // _enhancedTouchProvider ??= new EnhancedTouchProvider(this).AddTo(this);
            _inputTouchesProvider ??= new InputTouchesProvider(this).AddTo(this);
            return (_onPressed, _onReleased, _onMoved);
        }

        static Result ScreenToResult(Vector2 screenPosition, Camera camera)
        {
            var worldPosition = camera.ScreenToWorldPoint(screenPosition);
            return new Result { ScreenPosition = screenPosition, WorldPosition = worldPosition };
        }


        private class InputTouchesProvider : IDisposable
        {
            private DisposableBag _disposable;

            public InputTouchesProvider(InputPointerHandler handler)
            {
                var onTouch = handler.UpdateAsObservable()
                    .Where(static _ => Input.touches.Length > 0)
                    .Select(static _ => Input.touches[0]);

                var onChangedTouch = onTouch
                    .Select(touch => (touch.phase, touch.position))
                    .DistinctUntilChanged();

                var onPressed = onChangedTouch
                    .Where(touch => touch.phase == UnityEngine.TouchPhase.Began);

                var onReleased = onChangedTouch
                    .Where(touch => touch.phase is UnityEngine.TouchPhase.Ended or UnityEngine.TouchPhase.Canceled);

                var onMoved = onChangedTouch
                    .Where(touch => touch.phase == UnityEngine.TouchPhase.Moved);

                var onPressedState = (subject: handler._onPressed, camera: handler.targetCamera);
                var onReleasedState = (subject: handler._onReleased, camera: handler.targetCamera);
                var onMovedState = (subject: handler._onMoved, camera: handler.targetCamera);

                onPressed.Subscribe(onPressedState, static (touch, state) =>
                {
                    (Subject<Result> pressed, Camera camera) = state;
                    pressed.OnNext(ScreenToResult(touch.position, camera));
                }).AddTo(ref _disposable);

                onReleased.Subscribe(onReleasedState, static (touch, state) =>
                {
                    (Subject<Result> released, Camera camera) = state;
                    released.OnNext(ScreenToResult(touch.position, camera));
                }).AddTo(ref _disposable);

                onMoved.Subscribe(onMovedState, static (touch, state) =>
                {
                    (Subject<Result> moved, Camera camera) = state;
                    moved.OnNext(ScreenToResult(touch.position, camera));
                }).AddTo(ref _disposable);
            }

            public void Dispose()
            {
                _disposable.Dispose();
            }
        }

        private class EnhancedTouchProvider : IDisposable
        {
            private DisposableBag _disposable;

            public EnhancedTouchProvider(InputPointerHandler handler)
            {
                var onTouch = handler.UpdateAsObservable()
                    .Where(static _ => Touch.activeTouches.Count > 0)
                    .Select(static _ => Touch.activeTouches[0]);

                var onChangedTouch = onTouch
                    .Select(touch => (touch.phase, touch.screenPosition))
                    .DistinctUntilChanged();

                var onPressed = onChangedTouch
                    .Where(touch => touch.phase == TouchPhase.Began);

                var onReleased = onChangedTouch
                    .Where(touch => touch.phase is TouchPhase.Ended or TouchPhase.Canceled);

                var onMoved = onChangedTouch
                    .Where(touch => touch.phase == TouchPhase.Moved);

                var onPressedState = (subject: handler._onPressed, camera: handler.targetCamera);
                var onReleasedState = (subject: handler._onReleased, camera: handler.targetCamera);
                var onMovedState = (subject: handler._onMoved, camera: handler.targetCamera);

                onPressed.Subscribe(onPressedState, static (touch, state) =>
                {
                    (Subject<Result> pressed, Camera camera) = state;
                    pressed.OnNext(ScreenToResult(touch.screenPosition, camera));
                }).AddTo(ref _disposable);

                onReleased.Subscribe(onReleasedState, static (touch, state) =>
                {
                    (Subject<Result> released, Camera camera) = state;
                    released.OnNext(ScreenToResult(touch.screenPosition, camera));
                }).AddTo(ref _disposable);

                onMoved.Subscribe(onMovedState, static (touch, state) =>
                {
                    (Subject<Result> moved, Camera camera) = state;
                    moved.OnNext(ScreenToResult(touch.screenPosition, camera));
                }).AddTo(ref _disposable);
            }

            public void Dispose()
            {
                _disposable.Dispose();
            }
        }


        private class MouseInputProvider : IDisposable
        {
            private DisposableBag _disposable;

            public MouseInputProvider(InputPointerHandler handler)
            {
                var onMouse = handler.UpdateAsObservable()
                    .Where(static _ => Mouse.current.enabled)
                    .Select(static _ => Mouse.current);

                var onMouseButton =
                    onMouse.Select(mouse => (button: mouse.leftButton, position: mouse.position.ReadValue()));

                var onPressed = onMouseButton
                    .Where(mouse => mouse.button.wasPressedThisFrame);

                var onReleased = onMouseButton
                    .Where(mouse => mouse.button.wasReleasedThisFrame);

                var onMoved = onMouseButton
                    .Where(mouse => mouse.button.isPressed)
                    .DistinctUntilChangedBy(mouse => (mouse.position, Mouse.current.leftButton.isPressed))
                    .Skip(1);

                var onPressedState = (subject: handler._onPressed, camera: handler.targetCamera);
                var onReleasedState = (subject: handler._onReleased, camera: handler.targetCamera);
                var onMovedState = (subject: handler._onMoved, camera: handler.targetCamera);

                onPressed.Subscribe(onPressedState, static (mouse, state) =>
                {
                    (Subject<Result> pressed, Camera camera) = state;
                    pressed.OnNext(ScreenToResult(mouse.position, camera));
                }).AddTo(ref _disposable);

                onReleased.Subscribe(onReleasedState, static (mouse, state) =>
                {
                    (Subject<Result> released, Camera camera) = state;
                    released.OnNext(ScreenToResult(mouse.position, camera));
                }).AddTo(ref _disposable);

                onMoved.Subscribe(onMovedState, static (mouse, state) =>
                {
                    (Subject<Result> moved, Camera camera) = state;
                    moved.OnNext(ScreenToResult(mouse.position, camera));
                }).AddTo(ref _disposable);
            }

            public void Dispose()
            {
                _disposable.Dispose();
            }
        }

        public readonly struct Result
        {
            public Vector2 ScreenPosition { get; init; }
            public Vector3 WorldPosition { get; init; }
        }
    }
}
