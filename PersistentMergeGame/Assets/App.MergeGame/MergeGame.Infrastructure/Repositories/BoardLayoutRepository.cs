// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Internal.Entities;
using MergeGame.Core.Internal.Repositories;
using MergeGame.Infrastructure.Data;
using UnityEngine.AddressableAssets;

namespace MergeGame.Infrastructure.Repositories
{
    public class BoardLayoutRepository : IBoardLayoutRepository
    {
        public async UniTask<BoardLayout> GetStartingLayout(CancellationToken ct = default)
        {
            var asset = await Addressables.LoadAssetAsync<BoardLayoutAsset>("BoardLayout/Starting_2").Task;
            return asset.ToBoardLayout(1);
        }
    }
}
