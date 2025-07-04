// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using LitMotion;
using UnityEngine;

namespace App.MergeGame.Motion
{
    [CreateAssetMenu(menuName = "Merge Game/Motion/Block State Color Motion", fileName = "BlockStateColorMotion")]
    public class BlockStateColorMotion : ScriptableObject
    {
        [field: SerializeField] public Ease Ease { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
    }
}
