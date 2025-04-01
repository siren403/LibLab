// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter.VContainer;

namespace App.UI.Pages
{
    public static class PageExtensions
    {
        public static void RegisterPages(this IContainerBuilder builder)
        {
            builder.RegisterVitalRouter(routing =>
            {
                routing.Map<PagePresenter>();
            });
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
}
