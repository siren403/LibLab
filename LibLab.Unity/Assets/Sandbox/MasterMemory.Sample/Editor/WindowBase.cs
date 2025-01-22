// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEditor;
using UnityEngine.UIElements;

namespace MasterMemory.Sample.Editor
{
    internal abstract class WindowBase : EditorWindow
    {
        protected abstract string UxmlPath { get; }

        protected VisualElement Root => rootVisualElement;

        protected virtual void CreateGUI()
        {
            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            VisualElement root = rootVisualElement;
            root.Add(uxml.Instantiate());
        }
    }
}
