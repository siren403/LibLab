// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using App.MergeGame.Core.Dtos;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Commands.MergeBoards
{
    public struct GetBlocks : ICommand<BlockInfo[]>
    {
    }
}
