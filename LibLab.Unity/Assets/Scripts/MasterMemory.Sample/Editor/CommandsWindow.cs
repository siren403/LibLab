// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VitalRouter;
using VitalRouter.MRuby;

namespace MasterMemory.Sample.Editor
{
    [MRubyObject]
    internal partial struct DispatchCommand : ICommand
    {
        public string EventName;
    }

    [MRubyObject]
    internal partial struct AddCommand : ICommand
    {

    }

    [Routes]
    [Serializable]
    [MRubyCommand("dispatch", typeof(DispatchCommand))]
    [MRubyCommand("add", typeof(AddCommand))]
    internal partial class CommandsPresenter : MRubyCommandPreset, IPresenter
    {
        public int a;
        public int b;

        [CreateProperty]
        public int Result { get; private set; }

        [Route]
        private void On(AddCommand command)
        {
            Result = a + b;
        }

        [Route]
        private void On(DispatchCommand command)
        {
            Debug.Log($"DispatchCommand: {command.EventName}");
        }
    }

    internal interface IPresenter
    {
        Subscription MapTo(ICommandSubscribable subscribable);
        void UnmapRoutes();
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

    internal abstract class MMSampleWindow<TPresenter> : WindowBase
        where TPresenter : MRubyCommandPreset, IPresenter
    {
        private const string UxmlDirectory = "Assets/Scripts/MasterMemory.Sample/Editor";

        private MRubyContext _context;

        protected sealed override string UxmlPath => $"{UxmlDirectory}/{UxmlSrc}";
        protected abstract string UxmlSrc { get; }

        protected virtual Router ContextRouter => Router.Default;

        protected abstract TPresenter Presenter { get; }

        protected virtual void OnEnable()
        {
            _context = CreateContextInternal();
            Presenter.MapTo(ContextRouter);
        }

        protected virtual void OnDisable()
        {
            _context.Dispose();
            _context = null;
            Presenter.UnmapRoutes();
        }

        protected override void CreateGUI()
        {
            base.CreateGUI();
            Root.dataSource = Presenter;
            Root.RegisterCallback((ClickEvent e, Router router) =>
            {
                switch (e.target)
                {
                    case DispatchButton button:
                    {
                        router.PublishAsync(new DispatchCommand
                        {
                            EventName = button.EventName
                        });
                        break;
                    }
                    case CommandButton button:
                    {
                        FireScript($"cmd :{button.CommandName}");
                        break;
                    }
                }
            }, ContextRouter);
        }

        private MRubyContext CreateContextInternal()
        {
            MRubyContext context = MRubyContext.Create();
            context.Router = ContextRouter;
            context.CommandPreset = Presenter;
            return context;
        }

        protected void FireScript(string source)
        {
            if (_context == null) return;
            if (string.IsNullOrWhiteSpace(source)) return;

            try
            {
                using MRubyScript script = _context.CompileScript(source);
                script.RunAsync();
            }
            catch (Exception e)
            {
                OnScriptException(e);
                _context.Dispose();
                _context = CreateContextInternal();
                Debug.LogError("Script error. Context has been reset.");
            }
        }

        protected virtual void OnScriptException(Exception e)
        {
            Debug.LogError(e);
        }

        protected void LoadScript(string source)
        {
            _context?.Load(source);
        }
    }

    internal abstract class WindowBase : EditorWindow
    {
        protected abstract string UxmlPath { get; }

        protected VisualElement Root => rootVisualElement;

        protected virtual void CreateGUI()
        {
            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            VisualElement root = rootVisualElement;
            root.Add(uxml.Instantiate());
        }
    }

}
