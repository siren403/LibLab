using UnityEngine;

namespace App.Scenes.MergeGame
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite = null!;
        [SerializeField] private Color oddColor = Color.white;
        [SerializeField] private Color evenColor = Color.gray;

        public Vector2 Size
        {
            get { return sprite.transform.localScale; }
        }

        public void OnChangedPosition(Vector2Int position)
        {
            bool isEven = (position.x + position.y) % 2 == 0;
            sprite.color = isEven ? evenColor : oddColor;
        }
    }
}
