// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using App.UI.Pages;
using Cysharp.Threading.Tasks;

namespace App.Scenes.Modal
{
    public class AlertFetcher : IPageFetcher
    {
        private readonly AlertState _state;

        // TODO: ID Database with SourceGenerator
        public string Id => Ids.Pages.Alert;

        private readonly Queue<TimeSpan> _simulateDelay = new();

        public AlertFetcher(AlertState state)
        {
            _state = state;
            _simulateDelay.Enqueue(TimeSpan.FromSeconds(3));
            _simulateDelay.Enqueue(TimeSpan.Zero);
        }

        public async UniTask<FetchResult> Fetch(CancellationToken cancellationToken)
        {
            if (_simulateDelay.Count == 0)
            {
                _state.Success();
                return FetchResult.Ok;
            }

            TimeSpan delay = _simulateDelay.Dequeue();

            if (delay == TimeSpan.Zero)
            {
                _state.Fail();
                return FetchResult.Ok;
            }

            await UniTask.Delay(delay, cancellationToken: cancellationToken);
            return new FetchRedirect("error");
        }
    }
}
