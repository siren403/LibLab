// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Commands.MergeBoards;
using NUnit.Framework;
using UnityEngine.TestTools;
using VContainer;
using VExtensions.Mediator.Abstractions;

namespace MergeGame.Api.Tests
{
    public class CommandsTest
    {
        [UnityTest]
        public IEnumerator MergeBoardCommands() => UniTask.ToCoroutine(async () =>
        {
            var container = TestUtils.BuildApi();
            var mediator = container.Resolve<IMediator>();
            await mediator.ExecuteAsync(new CreateBoardCommand { Width = 10, Height = 10 });

            var size = await mediator.ExecuteAsync<GetBoardSizeCommand, (int, int)>(new GetBoardSizeCommand());
            Assert.AreEqual((10, 10), size, "Board size should match the created dimensions");
        });
    }
}
