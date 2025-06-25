// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.ValueObjects;
using UnityEngine;
using VitalRouter;

namespace App.Scenes.MergeGame.Commands
{
    public struct SpawnBlockCommand : ICommand
    {
        public EntityId Id { get; init; }
        public Vector2Int Position { get; init; }
    }
}
