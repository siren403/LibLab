// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using MasterMemory.Sample.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VitalRouter;
using VitalRouter.MRuby;

namespace MasterMemory.Sample.Editor
{

    [Routes]
    internal abstract partial class MMSampleWindow<TPresenter> : WindowBase
        where TPresenter : MRubyCommandPreset, IPresenter
    {
        private const string UxmlDirectory = "Assets/Sandbox/MasterMemory.Sample/Editor";

        protected readonly Router WindowRouter = new();

        private MRubyContext _context;

        protected sealed override string UxmlPath => $"{UxmlDirectory}/{UxmlSrc}";
        protected abstract string UxmlSrc { get; }

        protected abstract TPresenter Presenter { get; }

        protected virtual void OnEnable()
        {
            _context = CreateContextInternal();
            Presenter.MapTo(_context.Router);
            MapTo(_context.Router, new DropOrdering());
        }

        protected virtual void OnDisable()
        {
            UnmapRoutes();
            Presenter.UnmapRoutes();
            _context.Dispose();
            _context = null;

            WindowRouter.Dispose();
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
                }
            }, _context.Router);
        }

        private MRubyContext CreateContextInternal()
        {
            MRubyContext context = MRubyContext.Create(WindowRouter, Presenter);
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

        [Route(CommandOrdering.Drop)]
        private async ValueTask On(DispatchCommand cmd, PublishContext ctx)
        {
            MRubyContext ruby = ctx.MRuby();
            if (ruby == null) return;
            using MRubyScript script = ruby.CompileScript($"cmd :{cmd.EventName}");
            await script.RunAsync(ctx.CancellationToken);
        }
    }
}
