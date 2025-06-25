// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Commands.MergeBoards;
using MergeGame.Core.Internal.Repositories;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.MergeBoards
{
    internal class BoardSizeHandler : ICommandHandler<GetBoardSizeCommand, (int Width, int Height)>
    {
        private readonly IMergeBoardRepository _repository;

        public BoardSizeHandler(IMergeBoardRepository repository)
        {
            _repository = repository;
        }

        public UniTask<(int Width, int Height)> ExecuteAsync(GetBoardSizeCommand command, CancellationToken ct)
        {
            var boardSize = _repository.Size;
            return UniTask.FromResult(boardSize);
        }
    }
}
