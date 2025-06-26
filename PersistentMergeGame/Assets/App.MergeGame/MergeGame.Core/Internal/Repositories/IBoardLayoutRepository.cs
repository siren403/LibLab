// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using MergeGame.Core.Internal.Entities;

namespace MergeGame.Core.Internal.Repositories
{
    public interface IBoardLayoutRepository
    {
        UniTask<BoardLayout> GetStartingLayout(CancellationToken ct = default);
    }
}
