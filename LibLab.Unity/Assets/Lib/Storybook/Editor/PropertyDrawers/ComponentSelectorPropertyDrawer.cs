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

            VisualTreeAsset uxml =
                Resources.Load<VisualTreeAsset>(
                    $"{nameof(Storybook)}/{nameof(ComponentSelectorPropertyDrawer)}");

            VisualElement root = uxml.Instantiate();
            ComponentSelectorElement element = root.Q<ComponentSelectorElement>();

            element.Label = property.displayName;
            element.ComponentType = selector.SourceType;
            element.ComponentField.BindProperty(
                property.FindPropertyRelative("source")
            );
            element.LocationField.BindProperty(
                property.FindPropertyRelative("location")
            );
            element.LocationField.RegisterValueChangedCallback((e) =>
            {
                SourceLocation? location = e.newValue as SourceLocation?;
                if (location == SourceLocation.Manual)
                {
                    element.ComponentField.value = null;
                    return;
                }
                (bool success, Component result) = GetSource(location, component, selector.SourceType);
                element.ComponentField.value = result;
            });
            return root;
        }

        private (bool success, Component result) GetSource(
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
