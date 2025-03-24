// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Storybook.Editor.VisualElements
{
    [UxmlElement]
    public partial class ComponentSelectorElement : BoundaryElement
    {

        [UxmlAttribute("label")]
        private string _label;

        public string Label
        {
            get => _label;
            set
            {
                _label = value;
                LabelControl.text = _label;
                bool isEmpty = string.IsNullOrEmpty(_label);
                LabelControl.EnableInClassList("label--disabled", isEmpty);
            }
        }

        public Type ComponentType
        {
            set => ComponentField.objectType = value;
        }

        private Label LabelControl => this.Q<Label>(className: "label");
        public ObjectField ComponentField => this.Q<ObjectField>(className: "field__component");
        public EnumField LocationField => this.Q<EnumField>(className: "field__location");

        protected override void OnAttachToPanel(AttachToPanelEvent evt)
        {
            Label = _label;
        }


    }
}
