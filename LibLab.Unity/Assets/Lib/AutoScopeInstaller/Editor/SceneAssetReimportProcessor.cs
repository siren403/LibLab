using System.Linq;
using UnityEditor;

namespace AutoScopeInstaller.Editor
{
    public class SceneAssetReimportProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            var hasImportedScene = importedAssets.Any(path => path.EndsWith(".unity"));
            var hasDeletedScene = deletedAssets.Any(path => path.EndsWith(".unity"));
            var hasMovedScene = movedAssets.Any(path => path.EndsWith(".unity"));
            var hasMovedFromAssetPathScene = movedFromAssetPaths.Any(path => path.EndsWith(".unity"));

            if (hasImportedScene || hasDeletedScene || hasMovedScene || hasMovedFromAssetPathScene)
            {
                var taggedScenes = TaggedScenes.Instance;
                if (taggedScenes == null)
                {
                    return;
                }

                var hasFailedReference = false;
                foreach (var reference in taggedScenes.Scenes)
                {
                    var result = reference.Validate();
                    if (!hasFailedReference)
                    {
                        hasFailedReference = !result;
                    }
                }

                if (hasFailedReference)
                {
                    EditorUtility.SetDirty(taggedScenes);
                    AssetDatabase.SaveAssetIfDirty(taggedScenes);
                    EditorGUIUtility.PingObject(taggedScenes);
                }
            }
        }
    }
}