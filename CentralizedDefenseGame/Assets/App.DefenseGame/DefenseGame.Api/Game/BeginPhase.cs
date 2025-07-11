// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameKit.Common.Results;
using VitalRouter;
using Void = GameKit.Common.Results.Void;

namespace DefenseGame.Api.Game
{
    public class BeginPhase : PhaseBase<BeginPhase>
    {
        public BeginPhase(GameContext context) : base(context)
        {
        }

        public async UniTask<BattlePhase> RunAsync(CancellationToken ct)
        {
            await PublishAsync(
                new LeveledUpCommand<BeginPhase>(this)
                {
                    Skill1 = 1, Skill2 = 2, Skill3 = 3
                },
                ct);
            return new BattlePhase(Context);
        }

        public IDisposable OnLeveledUp(PublishContinuation<LeveledUpCommand<BeginPhase>> handler)
            => SubscribeAwait(handler);

        public UniTask<long> SelectSkillAsync(long skillId, CancellationToken ct = default)
        {
            return UniTask.FromResult(skillId);
        }
    }
}
