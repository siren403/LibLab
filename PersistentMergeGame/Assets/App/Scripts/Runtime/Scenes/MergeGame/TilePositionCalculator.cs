using UnityEngine;

namespace App.Scenes.MergeGame
{
    public struct TilePositionCalculator
    {
        private readonly Vector2 _tileSize;
        private readonly Vector2 _boardSize;
        private readonly Vector2 _origin;

        public TilePositionCalculator(Vector2 tileSize, Vector2 boardSize, Vector2 origin)
        {
            _tileSize = tileSize;
            _boardSize = boardSize;
            _origin = origin;
        }

        public Vector2 GetTilePosition(Vector2Int position)
        {
            return new Vector3(
                position.x * _tileSize.x + _tileSize.x * 0.5f - _boardSize.x * _origin.x,
                position.y * _tileSize.y + _tileSize.y * 0.5f - _boardSize.y * _origin.y
            );
        }
    }
}
