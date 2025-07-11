// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;

namespace DefenseGame.Api.Game
{
    public readonly struct LeveledUpCommand<TPhase> : IPhaseCommand<TPhase> where TPhase : IPhaseContext
    {
        public TPhase Phase { get; }
        public long Skill1 { get; init; }
        public long Skill2 { get; init; }
        public long Skill3 { get; init; }

        public LeveledUpCommand(TPhase phase)
        {
            Phase = phase;
            Skill1 = 0;
            Skill2 = 0;
            Skill3 = 0;
        }
    }
}
