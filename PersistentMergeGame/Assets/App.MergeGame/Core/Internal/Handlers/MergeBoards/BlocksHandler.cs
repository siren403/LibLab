// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using App.MergeGame.Core.Dtos;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Commands.MergeBoards;
using MergeGame.Core.Internal.Repositories;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.MergeBoards
{
    internal class BlocksHandler : ICommandHandler<GetBlocks, BlockInfo[]>
    {
        private readonly IMergeBoardRepository _repository;

        public BlocksHandler(IMergeBoardRepository repository)
        {
            _repository = repository;
        }

        public async UniTask<BlockInfo[]> ExecuteAsync(GetBlocks command, CancellationToken ct)
        {
            return Array.Empty<BlockInfo>();
        }
    }
}
