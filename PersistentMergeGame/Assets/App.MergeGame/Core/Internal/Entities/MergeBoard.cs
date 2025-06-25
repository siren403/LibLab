// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace MergeGame.Core.Internal.Entities
{
    internal class MergeBoard
    {
        public int Width { get; }
        public int Height { get; }

        private readonly Block[,] _blocks;

        public MergeBoard(int width, int height)
        {
            Width = width;
            Height = height;
            _blocks = new Block[width, height];
        }
    }
}
