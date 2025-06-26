// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;

namespace MergeGame.Api.Game
{
    public record CreateGameResponse(
        int StatusCode,
        Ulid SessionId,
        int Width,
        int Height
    ) : Response(StatusCode)
    {
        public static CreateGameResponse Ok(Ulid sessionId, int width, int height)
        {
            return new CreateGameResponse(0, sessionId, width, height);
        }

        public static CreateGameResponse Error()
        {
            return new CreateGameResponse(-1, Ulid.Empty, 0, 0);
        }
    }
}
