// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.UI.Pages
{
    public interface IPage
    {
        UniTask Show(CancellationToken cancellationToken = default);
        UniTask Hide(CancellationToken cancellationToken = default);
    }
}
