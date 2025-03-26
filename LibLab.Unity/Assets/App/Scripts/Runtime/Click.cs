// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SceneLauncher;
using SceneLauncher.VContainer;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using VitalRouter;

namespace App
{
    [DisallowMultipleComponent]
    public class Click : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string id;

        private AsyncLazy<Router> _router;

        private void Awake()
        {
            _router = new AsyncLazy<Router>(async () =>
            {
                IObjectResolver container = await PostLaunchLifetimeScope.GetLaunchedTask(gameObject.scene);
                return container.Resolve<Router>();
            });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }
            _ = PublishAsync(new ClickCommand
            {
                Id = id
            });
        }

        private async ValueTask PublishAsync(ClickCommand command)
        {
            Router router = await _router;
            await router.PublishAsync(command);
        }
    }
}
