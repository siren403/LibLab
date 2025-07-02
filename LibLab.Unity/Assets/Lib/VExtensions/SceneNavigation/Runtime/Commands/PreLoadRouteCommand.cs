// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using VitalRouter;

namespace VExtensions.SceneNavigation.Commands
{
    public partial struct PreLoadRouteCommand : ICommand
    {
        public string Path { get; init; }
    }
}
