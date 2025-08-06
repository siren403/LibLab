// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using App.Scenes;
using App.Scenes.UI;
using Cysharp.Threading.Tasks;
using LitMotion;
using Microsoft.Extensions.Logging;
using R3;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer.Unity;
using VExtensions.SceneNavigation;
using VExtensions.SceneNavigation.Extensions;
using VitalRouter;

namespace App
{
    public class AddressableRouterEntry : IStartable, IDisposable
    {
        private readonly Navigator _navigator;
        private readonly ILogger<AddressableRouterEntry> _logger;
        private readonly AddressableComponents _components;
        private readonly Router _router;
        private DisposableBag _disposables;

        public AddressableRouterEntry(
            Navigator navigator,
            ILogger<AddressableRouterEntry> logger,
            AddressableComponents components,
            Router router)
        {
            _navigator = navigator;
            _logger = logger;
            _components = components;
            _router = router;

            components.CheckUpdateButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha1)))
                .SubscribeAwait(async (_, cancellationToken) =>
                {
                    var fade = _components.ScreenFadeFeature!;
                    await LMotion.Create(0f, 1f, 0.3f).Bind(t => fade.Progress = t);

                    components.LogLabel!.text = nameof(AddressableExtensions.CheckForCatalogUpdates);
                    var updates = await AddressableExtensions.CheckForCatalogUpdates();
                    components.LogLabel!.text = updates.Status == AsyncOperationStatus.Succeeded
                        ? $"{updates.Result.Count} catalogs loaded."
                        : updates.OperationException.Message;
                    await UniTask.Delay(TimeSpan.FromSeconds(1));

                    await LMotion.Create(1f, 0f, 0.3f).Bind(t => fade.Progress = t);
                }, AwaitOperation.Drop)
                .AddTo(ref _disposables);
            components.UpdateButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha2)))
                .SubscribeAwait(async (_, cancellationToken) => { await _navigator.CheckForUpdates(); },
                    AwaitOperation.Drop)
                .AddTo(ref _disposables);

            components.ToButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha3)))
                .SubscribeAwait(async (_, cancellationToken) =>
                {
                    string? location = _navigator.LoadedLocation;
                    if (location == null)
                    {
                        return;
                    }

                    await _router.PublishAsync(TransitionScene.FadeCommand.In());
                    switch (location)
                    {
                        case "/":
                            await _navigator.To("/intro");
                            break;
                        case "/intro":
                            await _navigator.To("/startup");
                            break;
                        case "/startup":
                            await _navigator.To("/intro");
                            break;
                    }

                    await _router.PublishAsync(TransitionScene.FadeCommand.Out());
                }, AwaitOperation.Drop)
                .AddTo(ref _disposables);
            components.PopButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha4)))
                .Where(_ => _navigator.HasHistory)
                .SubscribeAwait(async (_, cancellationToken) =>
                    {
                        await _router.PublishAsync(TransitionScene.FadeCommand.In());
                        await _navigator.Back();
                        await _router.PublishAsync(TransitionScene.FadeCommand.Out());
                    },
                    AwaitOperation.Drop)
                .AddTo(ref _disposables);
        }

        public void Start()
        {
            if (!_navigator.IsInitialized)
            {
                _components.LogLabel!.text = $"{nameof(Navigator)} Initializing failed.";
                return;
            }

            _components.LogLabel!.text = $"{nameof(Navigator)} Initialized";
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
