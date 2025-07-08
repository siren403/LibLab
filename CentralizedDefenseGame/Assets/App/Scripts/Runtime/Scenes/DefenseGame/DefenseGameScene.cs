// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using VContainer;
using VContainer.Unity;

namespace App.Scenes.DefenseGame
{
    public class DefenseGameScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<DefenseGameEntryPoint>();
            builder.RegisterComponentInHierarchy<DefenseGamePresenter>();
        }
    }
}
