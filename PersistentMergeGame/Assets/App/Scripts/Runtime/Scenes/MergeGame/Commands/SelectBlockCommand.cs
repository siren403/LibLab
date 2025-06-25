// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;
using VitalRouter;

namespace App.Scenes.MergeGame.Commands
{
    public struct SelectBlockCommand : ICommand
    {
        public Vector2Int Position { get; init; }
    }
}
