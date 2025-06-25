// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using App.MergeGame.Core.Dtos;
using MergeGame.Core.Commands.MergeBoards;
using MergeGame.Core.Internal.Handlers.MergeBoards;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core
{
    public static class MediatorBuilderExtensions
    {
        public static void RegisterCommands(this IMediatorBuilder builder)
        {
            builder.RegisterCommand<CreateBoardCommand, CreateBoardHandler>();
            builder.RegisterCommand<GetBoardSizeCommand, BoardSizeHandler, (int, int)>();
            builder.RegisterCommand<GetBlocks, BlocksHandler, BlockInfo[]>();
        }
    }
}
