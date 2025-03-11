// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SourceSelector<>))]
public class SourceSelectorPropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(
        SerializedProperty property)
    {
        if (property.boxedValue is not SourceSelector selector)
        {
            return base.CreatePropertyGUI(property);
        }
        if (property.serializedObject.targetObject is not Component target)
        {
            return base.CreatePropertyGUI(property);
        }

        Type type = selector.SourceType;

        VisualElement root = new()
        {
            style =
            {
                flexDirection = FlexDirection.Row
            }
        };

        ObjectField objectField = new()
        {
            objectType = type,
            style =
            {
                width = 100
            }
        };
        objectField.BindProperty(property.FindPropertyRelative("source"));
        root.Add(objectField);

        EnumField enumField = new()
        {
            style =
            {
                width = 70
            }
        };
        enumField.BindProperty(property.FindPropertyRelative("location"));
        enumField.RegisterValueChangedCallback((e) =>
        {
            SourceLocation? location = e.newValue as SourceLocation?;
            if (location == SourceLocation.Manual)
            {
                objectField.value = null;
                return;
            }
            (bool success, Component result) = GetSource(location, target, type);
            objectField.value = result;
        });
        root.Add(enumField);
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
