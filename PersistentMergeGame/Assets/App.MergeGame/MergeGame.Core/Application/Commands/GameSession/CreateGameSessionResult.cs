// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;

namespace MergeGame.Core.Application.Commands.GameSession
{
    public record CreateGameSessionResult(
        bool IsSuccess,
        Ulid Value,
        string? Message = null
    ) : Result<Ulid>(IsSuccess, Value, Message);
}
