// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Application.Commands.Board;
using MergeGame.Core.Application.Data;
using MergeGame.Common;
using MergeGame.Core.Internal.Extensions;
using MergeGame.Core.Internal.Managers;
using MergeGame.Core.Internal.Repositories;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Core.Internal.Handlers.Board
{
    internal class MergeBlockHandler : ICommandHandler<MergeBlockCommand, Result<MergeBlockData>>
    {
        private readonly GameManager _manager;
        private readonly IMergeRuleRepository _repository;

        public MergeBlockHandler(GameManager manager, IMergeRuleRepository repository)
        {
            _manager = manager;
            _repository = repository;
        }

        public async UniTask<Result<MergeBlockData>> ExecuteAsync(MergeBlockCommand command, CancellationToken ct)
        {
            var board = _manager.GetBoardOrThrow(command.SessionId);
            var mergeResult = board.MergeBlock(command.FromPosition, command.ToPosition, _repository);

            switch (mergeResult)
            {
                case Ok<ValueObjects.MergeBlockData> (var (fromCell, toCell)):
                    var ok = Result<MergeBlockData>.Ok(new MergeBlockData(
                        BoardCell.FromEntity(fromCell),
                        BoardCell.FromEntity(fromCell) with { X = toCell.Position.X, Y = toCell.Position.Y },
                        BoardCell.FromEntity(toCell)
                    ));
                    return ok;
                case Error<ValueObjects.MergeBlockData>(_, var message):
                    return Result<MergeBlockData>.Error(message);
            }

            return Result<MergeBlockData>.Error($"Merge failed: {command}");
        }
    }
}
