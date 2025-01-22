// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using VitalRouter;

namespace MasterMemory.Sample.UI
{
    public interface IPresenter
    {
        Subscription MapTo(ICommandSubscribable subscribable);
        void UnmapRoutes();
    }
}
