// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnitGenerator;
using Unity.Mathematics;

namespace DefenseGame.Contracts.ValueObjects
{
    [UnitOf(typeof((float2 center, float radius)))]
    public partial struct DefenseZone
    {
        public float2 Center => value.center;
        public float Radius => value.radius;

        public bool IsInside(float2 point)
        {
            return math.distancesq(Center, point) <= Radius * Radius;
        }

        public void Deconstruct(out float2 center, out float radius)
        {
            center = value.center;
            radius = value.radius;
        }

        public static DefenseZone FromRadius(float radius)
        {
            return new DefenseZone((float2.zero, radius));
        }
    }
}
