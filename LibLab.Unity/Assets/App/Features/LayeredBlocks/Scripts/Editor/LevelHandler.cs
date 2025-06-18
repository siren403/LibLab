using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.VersionControl;
using UnityEngine;

namespace App.Features.LayeredBlocks.Editor
{
    public class LevelHandler
    {
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID)
        {
            string path = AssetDatabase.GetAssetPath(instanceID);
            if (AssetDatabase.GetMainAssetTypeAtPath(path) !=
                typeof(LevelAsset)) return false;

            string guid = AssetDatabase.AssetPathToGUID(path);
            foreach (var w in Resources.FindObjectsOfTypeAll<LevelAssetWindow>())
            {
                if (w.SelectedGuid == guid)
                {
                    w.Focus();
                    return true;
                }
            }

            var window = EditorWindow.CreateWindow<LevelAssetWindow>(typeof(LevelAssetWindow), typeof(SceneView));
            window.Initialize(guid);
            window.Focus();
            return true;
        }
    }
}
