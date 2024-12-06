using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AutoScopeInstaller
{
    [Serializable]
    public partial record SceneReference
    {
        [SerializeField] private string path;
        [SerializeField] private string tag;

        public string Path
        {
            get => path;
            init => path = value;
        }

        public string Tag
        {
            get => tag;
            init => tag = value;
        }
    }

#if UNITY_EDITOR
    partial record SceneReference
    {
        [SerializeField] private string guid;

        public string Guid
        {
            get => guid;
            init => guid = value;
        }

        public bool Validate()
        {
            var result = true;
            var findPath = AssetDatabase.GUIDToAssetPath(guid);
            if (findPath != path)
            {
                Debug.LogWarning($"Broken scene path. replaced new path\nOld: {path}\nNew: {findPath}");
                path = findPath;
                result = false;
            }

            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            if (asset == null)
            {
                Debug.LogError($"Invalid scene path. missing scene reference\n{path}");
                path = string.Empty;
                guid = string.Empty;
                result = false;
            }

            return result;
        }
    }
#endif
}