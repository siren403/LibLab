// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;

namespace App.Scenes.MergeGame
{
    public class BoardCellSelector
    {
        private readonly Vector2Int?[] _positions = new Vector2Int?[2];
        private int _index = 0;

        public bool Select(Vector2Int position, out (Vector2Int, Vector2Int) positions)
        {
            positions = default;
            if (_index == 0)
            {
                _positions[_index++] = position;
                return false;
            }

            _positions[_index] = position;
            positions = (_positions[0]!.Value, _positions[1]!.Value);
            Reset();
            return true;
        }

        public void Reset()
        {
            _positions[0] = null;
            _positions[1] = null;
            _index = 0;
        }
    }
}
