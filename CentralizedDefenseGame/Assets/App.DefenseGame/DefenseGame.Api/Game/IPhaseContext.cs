// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace DefenseGame.Api.Game
{
    public interface IPhaseContext
    {
        ILogger Logger { get; }
    }
}
