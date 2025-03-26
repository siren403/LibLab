// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App
{
    internal class MainScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            Debug.Log("Installing: " + builder.ApplicationOrigin);
            builder.Register<MainPerson>(Lifetime.Singleton).As<IPerson>();
            builder.RegisterEntryPoint<AddressableRouterEntry>();
        }
    }
}
