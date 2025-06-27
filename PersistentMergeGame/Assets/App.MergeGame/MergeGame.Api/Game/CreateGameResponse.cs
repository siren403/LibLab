using System;
using MergeGame.Contracts.Board;

namespace MergeGame.Api.Game
{
    public record CreateGameResponse(
        int StatusCode,
        Ulid SessionId,
        int Width,
        int Height,
        IBoardCell[] Cells
    ) : Response(StatusCode)
    {
        public static CreateGameResponse Ok(Ulid sessionId, int width, int height, IBoardCell[] cells)
        {
            return new CreateGameResponse(0, sessionId, width, height, cells);
        }

        public static CreateGameResponse Error()
        {
            return new CreateGameResponse(-1, Ulid.Empty, 0, 0, Array.Empty<IBoardCell>());
        }
    }
}
