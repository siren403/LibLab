using System;
using MergeGame.Contracts.Board;

namespace MergeGame.Api.Game.CreateGame
{
    public record CreateGameResponse(
        Ulid SessionId,
        int Width,
        int Height,
        IBoardCell[] Cells
    );
}
