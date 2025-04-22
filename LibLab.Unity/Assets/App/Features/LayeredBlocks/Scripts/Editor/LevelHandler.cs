using UnityEditor;
using UnityEditor.Callbacks;

namespace App.Features.LayeredBlocks.Editor
{
    public class LevelHandler
    {
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID)
        {
            if (AssetDatabase.GetMainAssetTypeAtPath(AssetDatabase.GetAssetPath(instanceID)) == typeof(LevelAsset))
            {
                LevelAssetWindow.ShowWindow();
                return true;
            }

            return false;
        }
    }
}
