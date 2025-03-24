// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine.UIElements;
using UnityEngine;

namespace Storybook.Editor.VisualElements
{
    public class BoundaryElement : VisualElement
    {
        protected BoundaryElement()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        protected virtual void OnAttachToPanel(AttachToPanelEvent evt)
        {
        }

        protected virtual void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
        }

    }
}
