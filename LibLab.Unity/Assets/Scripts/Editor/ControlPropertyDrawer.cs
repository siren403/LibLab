// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Control<,>), true)]
public class ControlPropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(
        SerializedProperty property)
    {
        if (property.boxedValue is not Control control)
        {
            return base.CreatePropertyGUI(property);
        }
        if (property.serializedObject.targetObject is not Component target)
        {
            return base.CreatePropertyGUI(property);
        }

        VisualElement root = new()
        {
            style =
            {
                flexDirection = FlexDirection.Row
            }
        };

        Label label = new()
        {
            text = property.displayName
        };
        PropertyField sourceField = new(property.FindPropertyRelative("source"));
        PropertyField valueField = new(property.FindPropertyRelative("value"))
        {
            dataSourceType = control.ValueType,
            label = string.Empty,
            style =
            {
                flexGrow = 1
            }
        };
        valueField.RegisterValueChangeCallback((e) =>
        {
            if (property.boxedValue is not Control changedControl)
            {
                return;
            }
            ChangeResult result = changedControl.OnValueChanged();
            if (result == ChangeResult.Success)
            {
                EditorUtility.SetDirty(changedControl.Source);
            }
        });

        root.Add(label);
        root.Add(valueField);
        root.Add(sourceField);

        return root;
    }
}
