// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;
using VitalRouter;

namespace App.Scenes.MergeGame.Commands;

public readonly struct UpdateBlockStateCommand : ICommand
{
    public Vector2Int Cell { get; init; }
    public short State { get; init; }
}
