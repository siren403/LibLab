// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEditor.UIElements;
using UnityEngine.UIElements;

[assembly: UxmlNamespacePrefix("MasterMemory.Sample.Editor", "mmse")]

namespace MasterMemory.Sample.Editor
{
    [UxmlElement]
    public partial class DispatchButton : Button
    {
        [UxmlAttribute("event")]
        public string EventName { get; private set; }
    }

    [UxmlElement]
    public partial class CommandButton : Button
    {
        [UxmlAttribute("command")]
        public string CommandName { get; private set; }
    }
}
