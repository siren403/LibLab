// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MasterMemory.Sample.UI;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using VitalRouter;
using VitalRouter.MRuby;

namespace MasterMemory.Sample.Editor
{
    [MRubyObject]
    internal partial struct AddCommand : ICommand
    {

    }

    [Routes]
    [Serializable]
    [MRubyCommand("add", typeof(AddCommand))]
    internal partial class CommandsPresenter : MRubyCommandPreset, IPresenter
    {
        public string status;
        public int a;
        public int b;

        [CreateProperty]
        public int Result { get; private set; }

        [Route]
        private async ValueTask On(AddCommand command)
        {
            int total = a + b;
            Result = 0;
            status = "Calculating...";
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            status = "Done!";
            Result = total;
        }
    }

    internal class CommandsWindow : MMSampleWindow<CommandsPresenter>
    {
        protected override string UxmlSrc => "CommandsWindow.uxml";
        protected override CommandsPresenter Presenter { get; } = new();

        [MenuItem("Window/MasterMemory/Commands")]
        public static void Open()
        {
            CommandsWindow window = GetWindow<CommandsWindow>();
            window.titleContent = new GUIContent("CommandsWindow");
            window.Show();
        }
    }

}
