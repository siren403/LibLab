// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.UI.Pages
{
    public interface IPageFetcher
    {
        string Id { get; }
        UniTask<FetchResult> Fetch(CancellationToken cancellationToken);
    }

    public record FetchResult(bool Success)
    {
        public static readonly FetchSuccess Ok = new();
        public static readonly FetchFailed Fail = new();
    }

    public record FetchSuccess() : FetchResult(true);

    public record FetchFailed() : FetchResult(false);

    public record FetchRedirect(string RedirectId) : FetchResult(true);
}
