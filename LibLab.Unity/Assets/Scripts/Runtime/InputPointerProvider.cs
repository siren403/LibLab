using System;
using System.Linq;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class InputPointerProvider : MonoBehaviour
{
    private readonly Subject<(string, Vector2)> _onPressed = new();
    private readonly Subject<(string, Vector2)> _onReleased = new();
    private readonly Subject<(string, Vector2)> _onMoved = new();

    private void Awake()
    {
        // Touch
        {
            var onTouch = this.UpdateAsObservable()
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

            onPressed.Subscribe(_onPressed, static (touch, pressed) =>
            {
                pressed.OnNext(($"{nameof(touch)} {nameof(pressed)}", touch.screenPosition));
            }).AddTo(this);

            onReleased.Subscribe(_onReleased, static (touch, released) =>
            {
                released.OnNext(($"{nameof(touch)} {nameof(released)}", touch.screenPosition));
            }).AddTo(this);

            onMoved.Subscribe(_onMoved, static (touch, moved) =>
            {
                moved.OnNext(($"{nameof(touch)} {nameof(moved)}", touch.screenPosition));
            }).AddTo(this);
        }

        // Mouse
        {
            var onMouse = this.UpdateAsObservable()
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

            onPressed.Subscribe(_onPressed, static (mouse, pressed) =>
            {
                pressed.OnNext(($"{nameof(mouse)} {nameof(pressed)}", mouse.position));
            }).AddTo(this);

            onReleased.Subscribe(_onReleased, static (mouse, released) =>
            {
                released.OnNext(($"{nameof(mouse)} {nameof(released)}", mouse.position));
            }).AddTo(this);

            onMoved.Subscribe(_onMoved, static (mouse, moved) =>
            {
                moved.OnNext(($"{nameof(mouse)} {nameof(moved)}", mouse.position));
            }).AddTo(this);
        }

        _onPressed.Subscribe(e => Debug.Log($"Pressed: {e.Item1} at {e.Item2}"))
            .AddTo(this);
        _onReleased.Subscribe(e => Debug.Log($"Released: {e.Item1} at {e.Item2}"))
            .AddTo(this);
        _onMoved.Subscribe(e => Debug.Log($"Moved: {e.Item1} to {e.Item2}"))
            .AddTo(this);
    }
}
