// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Storybook.Buttons;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter.VContainer;

namespace App
{
    public class ModalScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            Debug.Log("Installing: " + nameof(ModalScene));
            builder.RegisterComponentInHierarchy<PrimaryButton>();
            builder.Register<PageContainer>(Lifetime.Singleton).AsSelf();
            bool existsPerson = builder.Exists(
                typeof(IPerson),
                true,
                true);
            if (!existsPerson)
            {
                builder.Register<AlertPerson>(Lifetime.Singleton).As<IPerson>();
            }

            builder.RegisterVitalRouter((routing) => { routing.Map<UiPresenter>(); });
        }
    }

    public class PageContainer : Dictionary<string, Page>
    {

    }
}
