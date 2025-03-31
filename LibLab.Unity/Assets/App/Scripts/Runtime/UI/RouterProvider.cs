// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using App.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VitalRouter;

namespace App.UI
{
    public class RouterProvider : ContainerProvider
    {
        private readonly AsyncLazy<Router> _router;

        public RouterProvider(GameObject origin) : base(origin)
        {
            _router = new AsyncLazy<Router>(async () =>
            {
                IObjectResolver container = await GetContainer();
                return container.Resolve<Router>();
            });
        }

        public async ValueTask PublishAsync<T>(T command)
            where T : ICommand
        {
            Router router = await _router;
            await router.PublishAsync(command);
        }
    }
}
