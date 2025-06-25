// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;

namespace MergeGame.Core.Internal.Repositories
{
    internal interface IMergeBoardRepository
    {
        bool HasBoard { get; }
        (int Width, int Height) Size { get; }
        void CreateBoard(int width, int height);
    }
}
