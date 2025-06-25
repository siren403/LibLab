// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Commands.MergeBoards
{
    public struct CreateBoardCommand : ICommand
    {
        public int Width { get; init; }
        public int Height { get; init; }
    }
}
