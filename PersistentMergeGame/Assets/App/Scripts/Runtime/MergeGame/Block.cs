using R3;
using UnityEngine;

namespace App.MergeGame
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite = null!;

        public Color Color
        {
            set => sprite.color = value;
        }

        public void OnMovePosition()
        {
            sprite.sortingOrder = 1;
        }

        public void OnReturnPosition()
        {
            sprite.sortingOrder = 0;
        }
    }
}
