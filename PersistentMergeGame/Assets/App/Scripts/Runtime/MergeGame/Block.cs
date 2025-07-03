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

        public short State
        {
            set
            {
                switch (value)
                {
                    case 1:
                        Color = new Color(.5f, .5f, .5f, 1f);
                        break;
                    case 2:
                        Color = Color.white;
                        break;
                }
            }
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
