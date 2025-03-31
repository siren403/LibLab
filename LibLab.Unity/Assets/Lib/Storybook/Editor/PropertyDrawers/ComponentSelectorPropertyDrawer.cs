// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Storybook.Editor.VisualElements;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Storybook.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ComponentSelector<>))]
    public class ComponentSelectorPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.boxedValue is not ComponentSelector selector)
            {
                return base.CreatePropertyGUI(property);
            }
            if (property.serializedObject.targetObject is not Component component)
            {
                return base.CreatePropertyGUI(property);
            }

            VisualTreeAsset uxml = AssetLoader.LoadUxml<ComponentSelectorPropertyDrawer>();
            VisualElement root = uxml.Instantiate();
            ComponentSelectorElement element = root.Q<ComponentSelectorElement>();

            element.Label = property.displayName;
            Bind(element, selector.SourceType, property, component);
            return root;
        }

        public static void Bind(ComponentSelectorElement componentSelector, Type sourceType,
            SerializedProperty selectorProperty, Component component)
        {
            SerializedProperty sourceProperty = selectorProperty.FindPropertyRelative("source");
            SerializedProperty locationProperty = selectorProperty.FindPropertyRelative("location");
            componentSelector.ComponentType = sourceType;
            componentSelector.ComponentField.BindProperty(
                sourceProperty
            );
            componentSelector.ComponentField.RegisterValueChangedCallback(e =>
            {
                componentSelector.EnableInClassList("failed", componentSelector.ComponentField.value == null);
            });

            componentSelector.LocationField.BindProperty(
                locationProperty
            );

            componentSelector.LocationField.RegisterValueChangedCallback((e) =>
            {
                SourceLocation? location = e.newValue as SourceLocation?;
                if (location == SourceLocation.Manual)
                {
                    componentSelector.EnableInClassList("failed", componentSelector.ComponentField.value == null);
                    return;
                }
                (bool success, Component result) = GetSource(location, component, sourceType);
                componentSelector.ComponentField.value = result;
                componentSelector.EnableInClassList("failed", !success);
            });
        }

        private static (bool success, Component result) GetSource(
            SourceLocation? location,
            Component target,
            Type type)
        {
            switch (location)
            {
                case SourceLocation.Origin:
                {
                    bool success = target.TryGetComponent(type, out Component component);
                    return (success, component);
                }
                case SourceLocation.Children:
                {
                    Component component = target.GetComponentInChildren(type);
                    return (component != null, component);
                }
            }
            return (false, null);
        }
    }
}
