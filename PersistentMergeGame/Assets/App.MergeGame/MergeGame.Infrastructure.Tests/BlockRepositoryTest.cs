// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Internal.Repositories;
using MergeGame.Infrastructure.Extensions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;

namespace MergeGame.Infrastructure.Tests
{
    public class BlockRepositoryTest : SupabaseClientTestBase
    {
        [UnityTest]
        [TestCase(1, ExpectedResult = null!)]
        [TestCase(2, ExpectedResult = null!)]
        [TestCase(6, ExpectedResult = null!)]
        [TestCase(7, ExpectedResult = null!)]
        public IEnumerator GetStuffById(long id) => UniTask.ToCoroutine(async () =>
        {
            var bld = new ContainerBuilder();
            bld.RegisterRepositories();
            bld.RegisterInstance(await GetClient());
            var container = bld.Build();
            var repository = container.Resolve<IBlockRepository>();
            var stuff = await repository.GetBlock(id, Ct);
            Assert.IsNotNull(stuff);
            Debug.Log(stuff);
        });
    }
}
