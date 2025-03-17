// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;
using UnityEngine.UIElements;

namespace Storybook.Editor
{
    public static class AssetLoader
    {
        public static VisualTreeAsset LoadUxml<T>()
        {
            string editorName = typeof(T).Name;
            return Resources.Load<VisualTreeAsset>(
                $"{nameof(Storybook)}/{editorName}");

        }
    }
}
