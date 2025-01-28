// // Licensed to the.NET Foundation under one or more agreements.
// // The.NET Foundation licenses this file to you under the MIT license.
//
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.UIElements;
// using VitalRouter;
// using VitalRouter.MRuby;
//
// namespace MasterMemory.Sample.UI
// {
//     [Routes]
//     public abstract partial class RouterElement : VisualElement, IBindableRouter
//     {
//         private Router _router;
//
//         protected RouterElement()
//         {
//             RegisterClickEvent();
//         }
//
//         Router IBindableRouter.Router
//         {
//             set
//             {
//                 _router = value;
//                 RegisterClickEvent();
//             }
//         }
//
//         private void RegisterClickEvent()
//         {
//             if (!Application.isPlaying) return;
//             if (_router == null) return;
//             UnregisterCallback<ClickEvent>(OnClick);
//             RegisterCallback<ClickEvent>(OnClick);
//         }
//
//         private void OnClick(ClickEvent e)
//         {
//             switch (e.target)
//             {
//                 case DispatchButton button:
//                 {
//                     e.StopImmediatePropagation();
//                     _router.PublishAsync(new DispatchCommand
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


