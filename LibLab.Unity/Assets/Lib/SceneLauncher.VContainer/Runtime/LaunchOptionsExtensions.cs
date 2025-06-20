// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace SceneLauncher.VContainer
{
    public static class LaunchOptionsExtensions
    {
        public const string VContainerKey = "VContainer";

        public static void SetExtension(this LaunchOptions options, StartupLifetimeScope lifetimeScope)
        {
            options.Extensions[VContainerKey] = lifetimeScope;
        }
    }
}
