// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnitGenerator;

namespace MergeGame.Core.ValueObjects
{
    [UnitOf(typeof((int x, int y)))]
    public readonly partial struct Position
    {
        public Position(int x, int y)
        {
            value = (x, y);
        }

        public void Deconstruct(out int x, out int y)
        {
            (x, y) = AsPrimitive();
        }
    }
}
