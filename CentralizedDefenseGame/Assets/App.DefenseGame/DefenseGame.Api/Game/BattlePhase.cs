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
    public class BattlePhase : PhaseBase<BattlePhase>
    {
        public BattlePhase(GameContext context) : base(context)
        {
        }

        public async UniTask RunAsync(CancellationToken ct)
        {
            // 전투 시작
            // 적군 스폰 데이터 퍼블리시
            // 경험치 획득으로 레벨업 시 넘어감
            // 유닛 전부 사망 시 게임 종료
            await PublishAsync(
                new LeveledUpCommand<BattlePhase>(this)
                {
                    Skill1 = 1, Skill2 = 2, Skill3 = 3
                }, ct);

            Context.IsPlaying = false;
        }

        public IDisposable OnLeveledUp(PublishContinuation<LeveledUpCommand<BattlePhase>> handler)
        {
            return SubscribeAwait(handler);
        }

        public UniTask<long> SelectSkillAsync(long skillId, CancellationToken ct = default)
        {
            var result = FastResult<Ulid>.Fail($"{nameof(SelectSkillAsync)}.NotImplemented");

            if (result.IsError(out FastResult<Void> fail))
            {
                Context.Cancel(fail);
            }

            return UniTask.FromResult(skillId);
        }
    }
}
