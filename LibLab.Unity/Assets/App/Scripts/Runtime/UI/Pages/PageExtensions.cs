// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using Utf8StringInterpolation;
using VContainer;
using VContainer.Unity;
using VitalRouter.VContainer;
using ZLogger;
using ZLogger.Unity;

namespace App.UI.Pages
{
    public static class PageExtensions
    {
        public static void RegisterPages(this IContainerBuilder builder, Action<PageBuilder> configure)
        {
            var page = new PageBuilder(builder);
            configure(page);

            builder.RegisterVitalRouter(routing => { routing.Map<PagePresenter>(); });
            builder.Register<PageNavigator>(Lifetime.Singleton).AsSelf();
            builder.RegisterEntryPoint<InputBackOnPop>();
        }

        private class InputBackOnPop : ITickable
        {
            private readonly PageNavigator _navigator;

            public InputBackOnPop(PageNavigator navigator)
            {
                _navigator = navigator;
            }

            public void Tick()
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _navigator.Pop().Forget();
                }
            }
        }
    }

    public class PageBuilder
    {
        private readonly IContainerBuilder _builder;

        public PageBuilder(IContainerBuilder builder)
        {
            _builder = builder;
        }

        public void AddFetcher<T>() where T : IPageFetcher
        {
            _builder.Register<T>(Lifetime.Scoped).As<IPageFetcher>();
        }
    }
}
