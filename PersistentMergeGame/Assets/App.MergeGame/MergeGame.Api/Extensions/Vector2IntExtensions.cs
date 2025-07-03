// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.ValueObjects;
using UnityEngine;

namespace MergeGame.Api.Extensions
{
    public static class Vector2IntExtensions
    {
        public static Position ToValue(this Vector2Int vector)
        {
            return new Position(vector.x, vector.y);
        }
    }
}
