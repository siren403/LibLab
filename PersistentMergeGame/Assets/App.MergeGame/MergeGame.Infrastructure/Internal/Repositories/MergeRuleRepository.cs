// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using MergeGame.Common.Results;
using MergeGame.Core.Internal.Entities;
using MergeGame.Core.Internal.Repositories;
using MergeGame.Core.ValueObjects;

namespace MergeGame.Infrastructure.Internal.Repositories
{
    internal class MergeRuleRepository : IMergeRuleRepository
    {
        private readonly Dictionary<BlockId, MergeRule> _mergeRules = new()
        {
            { 0, new MergeRule(0, 0, 1) },
            { 1, new MergeRule(1, 1, 2) },
            { 2, new MergeRule(2, 2, 3) },
            { 3, new MergeRule(3, 3, 4) },
            { 4, new MergeRule(4, 4, 5) },
            { 5, new MergeRule(5, 5, 6) },
            { 6, new MergeRule(6, 6, 7) },
            { 7, new MergeRule(7, 7, 8) },
            { 8, new MergeRule(8, 8, 9) },
            { 9, new MergeRule(9, 9, 10) }
        };

        public Result<MergeRule> FindMergeRule(BlockId sourceId)
        {
            if (_mergeRules.TryGetValue(sourceId, out var rule))
            {
                return Result<MergeRule>.Ok(rule);
            }

            return Result<MergeRule>.Fail($"No merge rule found for source block ID: {sourceId}");
        }
    }
}
