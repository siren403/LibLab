// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.Internal.Entities;
using MergeGame.Core.Internal.Repositories;

namespace MergeGame.Infrastructure.Repositories
{
    public class MergeBoardRepository : IMergeBoardRepository
    {
        private const string SaveKey = nameof(MergeBoard);

        private MergeBoard? _board;

        public bool HasBoard => _board != null;

        public (int Width, int Height) Size
        {
            get { return (_board?.Width ?? 0, _board?.Height ?? 0); }
        }

        public void CreateBoard(int width, int height)
        {
            _board = new MergeBoard(width, height);
        }
    }
}
