// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;
using UnityEngine.UIElements;

namespace MasterMemory.Sample.UI
{
    public class RegisterDispatchEvent : EventBase<RegisterDispatchEvent>
    {
        public string EventName { get; set; }

        protected override void Init()
        {
            base.Init();
            EventName = null;
            bubbles = true;
        }
    }

    [UxmlElement]
    public partial class DispatchButton : Button
    {
        public DispatchButton()
        {
            if (!Application.isPlaying) return;
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        [UxmlAttribute("event")]
        public string EventName { get; private set; }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            using RegisterDispatchEvent e = RegisterDispatchEvent.GetPooled();
            e.EventName = EventName;
            e.target = this;
            SendEvent(e);
        }
    }

    [UxmlElement]
    public partial class CommandButton : Button
    {
        [UxmlAttribute("command")]
        public string CommandName { get; private set; }
    }
}
