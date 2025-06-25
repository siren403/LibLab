// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Responses;
using UnityEngine;
using UnityEngine.TestTools;

namespace MergeGame.Infrastructure.Tests
{
    public class SupabaseConnectTest : SupabaseClientTestBase
    {
        [UnityTest]
        public IEnumerator Connect() => UniTask.ToCoroutine(async () => { await GetClient(); });

        [UnityTest]
        public IEnumerator LoadTables() => UniTask.ToCoroutine(async () =>
        {
            var client = await GetClient();
            AssertTable(await client.From<Models.BlockGroup>().Get(Ct));
            AssertTable(await client.From<Models.BlockType>().Get(Ct));
            AssertTable(await client.From<Models.Block>().Get(Ct));
            AssertTable(await client.From<Models.BlockMerge>().Get(Ct));
            AssertTable(await client.From<Models.BlockSpawner>().Get(Ct));

            return;

            void AssertTable<T>(ModeledResponse<T> response) where T : BaseModel, new()
            {
                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Models);
                Assert.IsTrue(response.Models.Count > 0);
                foreach (var item in response.Models)
                {
                    Assert.IsNotNull(item);
                    Debug.Log(item);
                }
            }
        });
    }
}
