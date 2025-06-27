// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using MergeGame.Core.Application.Data;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Application.Commands.Board
{
    public readonly struct GetBoardCellsCommand : ICommand<BoardCell[]>
    {
        public Ulid SessionId { get; init; }
    }
}
