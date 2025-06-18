// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace App.Features.LayeredBlocks.Editor
{
    public class LevelAssetWindow : EditorWindow
    {
        [SerializeField] private string? selectedGuid;
        [SerializeField] private LevelAsset? levelAsset;
        public string? SelectedGuid => selectedGuid;


        [SerializeField] private string? id;

        public void Initialize(string guid)
        {
            selectedGuid = guid;
            levelAsset = AssetDatabase.LoadAssetAtPath<LevelAsset>(AssetDatabase.GUIDToAssetPath(selectedGuid));
            id = levelAsset.Id;
            rootVisualElement.Clear();
            BuildUI(rootVisualElement);
        }

        private void BuildUI(VisualElement root)
        {
            root.Add(new Label() { bindingPath = "id" });
            root.Add(new TextField("ID: ") { bindingPath = "id" });
            root.Add(new Button(() =>
            {
                if (levelAsset == null)
                {
                    return;
                }

                levelAsset.Id = id ?? string.Empty;
                EditorUtility.SetDirty(levelAsset);
                AssetDatabase.SaveAssets();
            }) { text = "Save" });

            root.Bind(new SerializedObject(this));
        }

        private void OnEnable()
        {

        }

        private void CreateGUI()
        {
            if (string.IsNullOrEmpty(selectedGuid))
            {
                return;
            }
            BuildUI(rootVisualElement);
        }
    }
}
