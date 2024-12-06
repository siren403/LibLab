using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AutoScopeInstaller.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferencePropertyDrawer : PropertyDrawer
    {
        private void Save(SerializedProperty property, SceneReference reference)
        {
            property.boxedValue = reference;
            property.serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssetIfDirty(property.serializedObject.targetObject);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.boxedValue is not SceneReference reference)
            {
                return new PropertyField(property);
            }

            var root = new VisualElement();

            var sceneField = new ObjectField
            {
                label = "Scene",
                objectType = typeof(SceneAsset)
            };

            if (!string.IsNullOrWhiteSpace(reference.Guid))
            {
                var path = AssetDatabase.GUIDToAssetPath(reference.Guid);
                if (reference.Path != path)
                {
                    Save(property, reference with
                    {
                        Path = path
                    });
                    Debug.LogWarning($"Broken scene path.\nreplaced new path {path}");
                }

                var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (asset == null)
                {
                    Save(property, new SceneReference());
                    Debug.LogError($"Invalid scene path.\n{path}");
                }

                sceneField.SetValueWithoutNotify(asset);
            }

            sceneField.RegisterValueChangedCallback(e =>
            {
                if (e.newValue == null)
                {
                    return;
                }

                var path = AssetDatabase.GetAssetPath(e.newValue);
                var giud = AssetDatabase.AssetPathToGUID(path);
                property.boxedValue = reference with
                {
                    Path = path,
                    Guid = giud
                };
                property.serializedObject.ApplyModifiedProperties();
            });

            root.Add(sceneField);

            var tagField = new TextField
            {
                label = "Tag",
                isDelayed = true,
                value = reference.Tag
            };
            tagField.RegisterValueChangedCallback(e =>
            {
                Save(property, reference with
                {
                    Tag = e.newValue
                });
            });

            root.Add(tagField);

            return root;
        }
    }
}