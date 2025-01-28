// // Licensed to the.NET Foundation under one or more agreements.
// // The.NET Foundation licenses this file to you under the MIT license.
//
// using System;
// using VitalRouter;
// using VitalRouter.MRuby;
//
// namespace MasterMemory.Sample.UI
// {
//     public class PageContext : IDisposable
//     {
//
//         private bool _isDisposed;
//
//         private Router _manualRouter;
//
//         private object _origin;
//
//         private MRubyContext _ruby;
//
//         private PageContext()
//         {
//
//         }
//
//         public Router Router => _ruby.Router;
//         public IPresenter Presenter => _ruby.CommandPreset as IPresenter;
//
//         public void Dispose()
//         {
//             Presenter?.UnmapRoutes();
//
//             _ruby?.Dispose();
//             _ruby = null;
//
//             _manualRouter?.Dispose();
//             _manualRouter = null;
//         }
//
//         public void Dispose(object origin)
//         {
//             if (_isDisposed) return;
//             if (_origin != origin) return;
//             Dispose();
//         }
//
//         public static PageContext Create<TPresenter>(object origin, MRubyContext ruby, TPresenter presenter,
//             Router router = null)
//             where TPresenter : MRubyCommandPreset, IPresenter
//         {
//             ruby.CommandPreset = presenter;
//
//             if (router != null)
//             {
//                 ruby.Router = router;
//             }
//
//             return new PageContext
//             {
//                 _ruby = ruby, _origin = origin, _manualRouter = router
//             };
//         }
//     }
// }


