// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Cysharp.Threading.Tasks;

namespace App.UI.Indicators
{
    public interface IDeterminate
    {
        UniTask OnBegin();
        void OnProcessing();
        UniTask OnEnd();
    }
}
