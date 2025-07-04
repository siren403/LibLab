using App.MergeGame.Motion;
using LitMotion;
using UnityEngine;

namespace App.MergeGame
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite = null!;
        [SerializeField] private BlockStateColorMotion stateColorMotion = null!;

        public Color Color
        {
            set => sprite.color = value;
        }

        private MotionHandle _stateColorMotionHandle;

        public short State
        {
            set
            {
                var toColor = value switch
                {
                    1 => new Color(0.5f, 0.5f, 0.5f, 1f), // Gray
                    2 => Color.white, // White
                    _ => sprite.color
                };

                var fromColor = sprite.color;

                if (_stateColorMotionHandle.IsPlaying())
                {
                    _stateColorMotionHandle.TryCancel();
                }

                _stateColorMotionHandle = LMotion.Create(fromColor, toColor, stateColorMotion.Duration)
                    .WithEase(stateColorMotion.Ease)
                    .Bind(color => Color = color)
                    .AddTo(this);
            }
        }

        public void OnDragPosition()
        {
            sprite.sortingOrder = 1;
        }

        public void OnReturnPosition()
        {
            sprite.sortingOrder = 0;
        }
    }
}
