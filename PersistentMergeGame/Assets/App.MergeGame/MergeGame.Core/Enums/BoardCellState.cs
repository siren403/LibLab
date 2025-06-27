// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace MergeGame.Core.Enums
{
    public enum BoardCellState
    {
        Untouchable, // 선택 불가능한 블록
        Mergeable, // 병합 가능한 블록
        Movable, // 이동 가능한 블록
    }
}
