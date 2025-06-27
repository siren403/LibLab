using UnityEngine;
using VitalRouter;

namespace App.Scenes.MergeGame.Commands
{
    public struct SelectTileCommand : ICommand
    {
        public Vector2Int CellPosition { get; init; }
        public Vector2 WorldPosition { get; init; }
    }
}
