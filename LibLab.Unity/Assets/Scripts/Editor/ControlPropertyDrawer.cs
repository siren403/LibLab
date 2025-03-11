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

        SerializedProperty sourceProperty = property.FindPropertyRelative("source");

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
        root.Add(label);

        PropertyField valueField = new()
        {
            dataSourceType = control.ValueType
        };
        valueField.RegisterValueChangeCallback((e) =>
        {
            Debug.Log($"{e.changedProperty.boolValue} | {control}");
            control.OnValueChanged();
        });
        valueField.BindProperty(property.FindPropertyRelative("value"));
        root.Add(valueField);

        PropertyField sourceField = new(property.FindPropertyRelative("source"));
        root.Add(sourceField);

        return root;
    }
}
