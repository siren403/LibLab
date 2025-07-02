using System;
using R3;
using UnityEngine;

namespace App.MergeGame
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite = null!;
        [SerializeField] private Color oddColor = Color.white;
        [SerializeField] private Color evenColor = Color.gray;
        [SerializeField] private BoxCollider2D box = null!;

        public Vector2 Size
        {
            get { return sprite.size; }
        }

        private void Awake()
        {
            box.size = Size;
        }

        public void OnChangedPosition(Vector2Int position)
        {
            bool isEven = (position.x + position.y) % 2 == 0;
            sprite.color = isEven ? evenColor : oddColor;
        }
    }
}
