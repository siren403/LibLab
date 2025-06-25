using VitalRouter;

namespace App.Scenes.MergeGame.Commands
{
    public struct SpawnTilesCommand : ICommand
    {
        public int Width { get; init; }
        public int Height { get; init; }
    }
}
