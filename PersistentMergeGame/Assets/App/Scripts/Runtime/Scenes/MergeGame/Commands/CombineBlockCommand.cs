// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;
using VitalRouter;

namespace App.Scenes.MergeGame.Commands;

public readonly struct CombineBlockCommand : ICommand
{
    public Vector2Int FromPosition { get; init; }
    public Vector2Int ToPosition { get; init; }
}
