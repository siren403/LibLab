// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Internal.Managers;
using MergeGame.Core.ValueObjects;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board
{
    internal class GetBoardSizeHandler : ICommandHandler<GetBoardSizeCommand, FastResult<BoardSize>>
    {
        private readonly GameManager _manager;

        public GetBoardSizeHandler(GameManager manager)
        {
            _manager = manager;
        }

        public UniTask<FastResult<BoardSize>> ExecuteAsync(GetBoardSizeCommand command, CancellationToken ct)
        {
            var sessionId = command.SessionId;
            var result = _manager.GetSession(sessionId);

            if (result.IsError(out FastResult<BoardSize> fail))
            {
                return fail;
            }

            var board = _manager.GetBoard(result.Value);
            return FastResult<BoardSize>.Ok(new BoardSize(board.Width, board.Height));
        }
    }
}
