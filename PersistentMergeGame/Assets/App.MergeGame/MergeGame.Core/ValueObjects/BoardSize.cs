// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnitGenerator;

namespace MergeGame.Core.ValueObjects
{
    [UnitOf(typeof((int width, int height)))]
    public readonly partial struct BoardSize
    {
        public static BoardSize Zero => new((0, 0));

        public BoardSize(int width, int height)
        {
            value = (width, height);
        }

        public void Deconstruct(out int width, out int height)
        {
            (width, height) = value;
        }
    }
}
