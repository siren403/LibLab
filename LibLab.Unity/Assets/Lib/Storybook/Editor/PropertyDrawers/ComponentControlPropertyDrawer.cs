// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Storybook.Editor.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Storybook.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ComponentControl<,>), true)]
    public class ComponentControlPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.boxedValue is not IControl control)
            {
                Debug.Log("control is null");
                return base.CreatePropertyGUI(property);
            }
            if (property.serializedObject.targetObject is not Component component)
            {
                Debug.Log("component is null");
                return base.CreatePropertyGUI(property);
            }

            VisualTreeAsset uxml = AssetLoader.LoadUxml<ComponentControlPropertyDrawer>();
            VisualElement root = uxml.Instantiate();

            ComponentSelectorElement element = root.Q<ComponentSelectorElement>();
            SerializedProperty selectorProperty = property.FindPropertyRelative("selector");

            ComponentSelectorPropertyDrawer.Bind(element, control.SourceType, selectorProperty, component);
            element.ComponentField.RegisterValueChangedCallback((e) =>
            {
                OnValueChanged();
            });
            SerializedProperty valueProperty = property.FindPropertyRelative("value");
            Label label = root.Q<Label>(className: "component-control__label");
            label.text = property.displayName;
            PropertyField valueField = root.Q<PropertyField>(className: "component-control__field");
            valueField.BindProperty(valueProperty);
            valueField.RegisterValueChangeCallback((e) =>
            {
                OnValueChanged();
            });

            Foldout foldout = root.Q<Foldout>(className: "component-control__foldout");
            foldout.RegisterValueChangedCallback((e) =>
            {
                OnFoldoutChanged(e.newValue);
            });
            OnFoldoutChanged(foldout.value);

            void OnFoldoutChanged(bool value)
            {
                VisualElement selector = root.Q<VisualElement>(className: "component-control__selector");
                selector.EnableInClassList("hide", !value);
            }

            return root;

            void OnValueChanged()
            {
                if (property.boxedValue is not IControl changedControl)
                {
                    return;
                }
                ChangeResult result = changedControl.OnValueChanged();
                if (result == ChangeResult.Success)
                {
                    EditorUtility.SetDirty(changedControl.Source);
                }
                VisualElement componentControl = root.Q<VisualElement>(className: "component-control");
                componentControl.EnableInClassList("failed", result == ChangeResult.Failure);
            }
        }
    }
}
