// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using App.Navigation;
using App.Scenes;
using Cysharp.Threading.Tasks;
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

            AsyncLazy readyCache = new(async () => { await UniTask.WaitUntil(() => Caching.ready); });
            List<string> catalogs = new List<string>();


            components.CheckUpdateButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha1)))
                .SubscribeAwait(async (_, cancellationToken) =>
                {
                    components.LogLabel!.text = nameof(AddressableExtensions.CheckForCatalogUpdates);
                    var updates = await AddressableExtensions.CheckForCatalogUpdates();
                    catalogs.Clear();
                    if (updates.Status == AsyncOperationStatus.Succeeded)
                    {
                        catalogs.AddRange(updates.Result);
                        components.LogLabel!.text = $"{catalogs.Count} catalogs loaded.";
                    }
                    else
                    {
                        components.LogLabel!.text = updates.OperationException.Message;
                    }
                }, AwaitOperation.Drop)
                .AddTo(ref _disposables);
            components.UpdateButton!.OnClickAsObservable()
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha2)))
                .SubscribeAwait(async (_, cancellationToken) =>
                {
                    components.LogLabel!.text = nameof(Addressables.UpdateCatalogs);
                    await readyCache;
                    if (catalogs.Count == 0)
                    {
                        _logger.LogWarning("Empty catalog list.");
                        return;
                    }

                    await Addressables.UpdateCatalogs(true, catalogs);
                    components.LogLabel!.text = $"{catalogs.Count} catalogs updated.";
                    catalogs.Clear();
                }, AwaitOperation.Drop)
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
