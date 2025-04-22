// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace App.Features.LayeredBlocks.Editor
{
    public class LevelAssetWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            var window = GetWindow<LevelAssetWindow>();
            window.titleContent = new GUIContent("Level");
            window.Show();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.Add(new Label("Level Editor"));
        }
    }
}
