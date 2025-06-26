// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Infrastructure.Data;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace MergeGame.Infrastructure.Tests
{
    public class LdtkBoardLayoutAssetTest
    {
        [Test]
        public void ToBoardLayout()
        {
            var asset = AssetDatabase.LoadAssetAtPath<BoardLayoutAsset>("Assets/App/Data/BoardLayout.asset");
            var layout = asset.ToBoardLayout(1);
            Debug.Log(layout);
        }
    }
}
