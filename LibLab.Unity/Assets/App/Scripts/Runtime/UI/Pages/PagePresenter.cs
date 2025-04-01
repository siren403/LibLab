// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.UI.Working;
using VitalRouter;

namespace App.UI.Pages
{
    [Routes(CommandOrdering.Drop)]
    public partial class PagePresenter
    {
        private readonly PageNavigator _navigator;
        private readonly Router _router;
        private readonly Dictionary<string, IPageFetcher> _fetchers;

        public PagePresenter(PageNavigator navigator, IReadOnlyList<IPageFetcher> fetchers, Router router)
        {
            _navigator = navigator;
            _router = router;
            _fetchers = fetchers.ToDictionary(req => req.Id, req => req);
        }

        [Route]
        private async ValueTask OnPush(PushCommand command, PublishContext context)
        {
            string pushId = command.Id;
            if (_fetchers.TryGetValue(pushId, out var fetcher))
            {
                using var work = _router.ReadyWork();
                work.Begin();
                FetchResult result = await fetcher.Fetch(context.CancellationToken);
                switch (result)
                {
                    // TODO: global fallback or force redirect
                    case FetchFailed:
                        return;
                    case FetchRedirect(var redirectId):
                        pushId = redirectId;
                        break;
                }
            }
            await _navigator.Push(pushId, context.CancellationToken);
        }

        [Route]
        private async ValueTask OnPop(PopCommand command, PublishContext context)
        {
            await _navigator.Pop(context.CancellationToken);
        }
    }
}
