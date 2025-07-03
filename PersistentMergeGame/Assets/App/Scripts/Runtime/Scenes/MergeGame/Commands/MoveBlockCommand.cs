// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;
using VitalRouter;

namespace App.Scenes.MergeGame.Commands;

public readonly struct MoveBlockCommand : ICommand
{
    public Vector2Int FromCell { get; init; }
    public Vector2Int ToCell { get; init; }
}
