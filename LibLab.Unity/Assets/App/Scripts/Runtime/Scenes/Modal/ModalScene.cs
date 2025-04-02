// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using App.UI.Pages;
using App.UI.Working;
using VContainer;
using VContainer.Unity;

namespace App.Scenes.Modal
{
    public class ModalScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterPages(page =>
            {
                page.AddFetcher<AlertFetcher>();
            });
            builder.RegisterWorking(working => { working.ExpectDuration = TimeSpan.FromSeconds(0.3f); });

            builder.Register<AlertState>(Lifetime.Singleton).AsSelf();
        }
    }
}
