// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using GameKit.Common.Results;
using MergeGame.Core.Internal.Entities;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Core.Internal.Repositories;

internal interface IMergeRuleRepository
{
    FastResult<MergeRule> FindMergeRule(BlockId sourceId);
}
