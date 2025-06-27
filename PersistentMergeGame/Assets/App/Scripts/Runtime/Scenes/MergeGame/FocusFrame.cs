using System;
using LitMotion.Animation;
using UnityEngine;

namespace App.Scenes.MergeGame
{
    public class FocusFrame : MonoBehaviour
    {
        [SerializeField] private LitMotionAnimation motion = null!;
        [SerializeField] private SpriteRenderer sprite = null!;

        private Vector2 _latestShowPosition;

        private void Awake()
        {
            Hide();
        }

        public void Show(Vector2 position)
        {
            transform.position = position;
            _latestShowPosition = position;
            motion.Stop();
            motion.Play();
        }

        public void Hide()
        {
            motion.Stop();
            transform.localScale = Vector3.zero;
            sprite.color = new Color(1, 1, 1, 0);
        }

        public void Restore()
        {
            Show(_latestShowPosition);
        }
    }
}
