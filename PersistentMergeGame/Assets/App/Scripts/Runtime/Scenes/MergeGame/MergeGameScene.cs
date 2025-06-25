// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Api;
using VContainer;
using VContainer.Unity;
using VitalRouter.VContainer;

namespace App.Scenes.MergeGame
{
    public class MergeGameScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterMergeGame();
            builder.RegisterEntryPoint<MergeGameEntryPoint>();
            builder.RegisterVitalRouter(routing =>
            {
                routing.MapComponentInHierarchy<MergeGamePresenter>();
            });
        }
    }
}
