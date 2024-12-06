using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VContainer.Unity;

namespace AutoScopeInstaller
{
    public class TaggedScenes : ScriptableObject
    {
        [SerializeField] private SceneReference[] scenes;

        public IReadOnlyCollection<SceneReference> Scenes => scenes;

        #region Setup shared instance

        public static TaggedScenes Instance { get; private set; }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Auto Scope Installer/Tagged Scenes")]
        public static void CreateAsset()
        {
            var path = EditorUtility.SaveFilePanelInProject(
                "Save TaggedScenes",
                "TaggedScenes",
                "asset",
                string.Empty);

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var newSettings = CreateInstance<TaggedScenes>();
            AssetDatabase.CreateAsset(newSettings, path);

            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.RemoveAll(x => x is TaggedScenes);
            preloadedAssets.Add(newSettings);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }

        private static void LoadInstanceFromPreloadAssets()
        {
            var preloadAsset = PlayerSettings.GetPreloadedAssets()
                .FirstOrDefault(x => x is VContainerSettings);
            if (preloadAsset is TaggedScenes instance)
            {
                instance.OnDisable();
                instance.OnEnable();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInitialize()
        {
            // For editor, we need to load the Preload asset manually.
            LoadInstanceFromPreloadAssets();
        }
#endif

        private void OnEnable()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            Instance = null;
        }

        #endregion

    }
}