// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using App.Navigation;
using App.Scenes;
using Cysharp.Threading.Tasks;
using LitMotion;
using Microsoft.Extensions.Logging;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer.Unity;

namespace App
{
    public class AddressableRouterEntry : IAsyncStartable, IDisposable
    {
        private readonly SceneNavigator _navigator;
        private readonly ILogger<AddressableRouterEntry> _logger;
        private readonly AddressableComponents _components;
        private DisposableBag _disposables;

        public AddressableRouterEntry(
            SceneNavigator navigator,
            ILogger<AddressableRouterEntry> logger,
            AddressableComponents components)
        {
            _navigator = navigator;
            _logger = logger;
            _components = components;

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

            components.PushButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha3)))
                .SubscribeAwait(async (_, cancellationToken) =>
                {
                    string? location = _navigator.LoadedLocation;
                    if (location == null)
                    {
                        return;
                    }
                    var fade = _components.ScreenFadeFeature!;
                    await LMotion.Create(0f, 1f, 0.3f).Bind(t => fade.Progress = t);
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
                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                    await LMotion.Create(1f, 0f, 0.3f).Bind(t => fade.Progress = t);

                }, AwaitOperation.Drop)
                .AddTo(ref _disposables);
            components.PopButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha4)))
                .SubscribeAwait(async (_, cancellationToken) => { await _navigator.Back(); },
                    AwaitOperation.Drop)
                .AddTo(ref _disposables);
        }

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            await _navigator.Initialize();

            if (!_navigator.IsInitialized)
            {
                _components.LogLabel!.text = $"{nameof(SceneNavigator)} Initializing failed.";
                return;
            }

            _components.LogLabel!.text = $"{nameof(SceneNavigator)} Initialized";
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
