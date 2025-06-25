// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Commands.MergeBoards;
using MergeGame.Core.Internal.Repositories;
using Microsoft.Extensions.Logging;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.MergeBoards
{
    internal class CreateBoardHandler : ICommandHandler<CreateBoardCommand>
    {
        private readonly ILogger<CreateBoardHandler> _logger;
        private readonly IMergeBoardRepository _repository;

        public CreateBoardHandler(ILogger<CreateBoardHandler> logger, IMergeBoardRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public UniTask ExecuteAsync(CreateBoardCommand command, CancellationToken ct)
        {
            _repository.CreateBoard(command.Width, command.Height);
            _logger.LogInformation(
                "Creating a new board with dimensions {Width}x{Height}",
                command.Width, command.Height
            );
            return UniTask.FromResult<string>("Board created successfully");
        }
    }
}
