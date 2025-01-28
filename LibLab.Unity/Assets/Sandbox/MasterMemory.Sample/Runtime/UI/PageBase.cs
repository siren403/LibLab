// // Licensed to the.NET Foundation under one or more agreements.
// // The.NET Foundation licenses this file to you under the MIT license.
//
// using System;
// using System.Threading.Tasks;
// using Unity.Properties;
// using UnityEngine;
// using UnityEngine.UIElements;
// using VitalRouter;
// using VitalRouter.MRuby;
//
// namespace MasterMemory.Sample.UI
// {
//     public class DataSource
//     {
//         [CreateProperty]
//         public object Presenter { get; init; }
//
//         [CreateProperty]
//         public object Element { get; init; }
//     }
//
//     [Routes]
//     public abstract partial class PageBase<TPresenter> : VisualElement
//         where TPresenter : MRubyCommandPreset, IPresenter
//     {
//
//         private PageContext _context;
//
//         protected PageBase()
//         {
//             RegisterEvents();
//         }
//
//         private bool IsPlaying => Application.isPlaying;
//
//         private void RegisterEvents()
//         {
//             if (!IsPlaying) return;
//             RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
//             RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
//             RegisterCallback<ClickEvent>(OnClick);
//         }
//
//         protected virtual PageContext CreateContext()
//         {
//             return null;
//         }
//
//         protected void UseContext(PageContext context)
//         {
//             if (_context != null)
//             {
//                 throw new InvalidOperationException("Context already set.");
//             }
//
//             _context = context;
//         }
//
//         private void OnAttachToPanel(AttachToPanelEvent e)
//         {
//             _context = CreateContext();
//             if (_context == null) return;
//
//             _context.Presenter.MapTo(_context.Router);
//             MapTo(_context.Router, new DropOrdering());
//             dataSource = new DataSource
//             {
//                 Presenter = _context.Presenter, Element = this
//             };
//
//             if (!string.IsNullOrWhiteSpace(name))
//             {
//                 ContextBinding.Contexts[name] = _context;
//                 ContextBinding.Routers[name] = _context.Router;
//             }
//         }
//
//         private void OnDetachFromPanel(DetachFromPanelEvent e)
//         {
//             if (_context == null) return;
//
//             if (!string.IsNullOrWhiteSpace(name))
//             {
//                 ContextBinding.Contexts.Remove(name);
//                 ContextBinding.Routers.Remove(name);
//             }
//
//             UnmapRoutes();
//             _context.Dispose(this);
//             _context = null;
//         }
//
//         private void OnClick(ClickEvent e)
//         {
//             switch (e.target)
//             {
//                 case DispatchButton button:
//                 {
//                     e.StopImmediatePropagation();
//                     _context.Router.PublishAsync(new DispatchCommand
//                     {
//                         EventName = button.EventName
//                     });
//                     break;
//                 }
//             }
//         }
//
//         [Route(CommandOrdering.Drop)]
//         private async ValueTask On(DispatchCommand cmd, PublishContext ctx)
//         {
//             MRubyContext ruby = ctx.MRuby();
//             if (ruby == null) return;
//             using MRubyScript script = ruby.CompileScript($"cmd :{cmd.EventName}");
//             await script.RunAsync(ctx.CancellationToken);
//         }
//     }
// }


