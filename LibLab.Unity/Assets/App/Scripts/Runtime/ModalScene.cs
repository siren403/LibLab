// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

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
            builder.RegisterEntryPoint<Init>();
            builder.RegisterComponentInHierarchy<PrimaryButton>();
            bool existsPerson = builder.Exists(
                typeof(IPerson),
                true,
                true);
            if (!existsPerson)
            {
                builder.Register<AlertPerson>(Lifetime.Singleton).As<IPerson>();
            }
            builder.RegisterVitalRouter((routing) =>
            {
                routing.Map<UiPresenter>();
            });
        }

        private class Init : IInitializable
        {
            private PrimaryButton _button;
            private IPerson _person;

            public Init(PrimaryButton button, IPerson person)
            {
                _button = button;
                _person = person;
            }

            public void Initialize()
            {
                _button.Label = _person.Name;
            }
        }
    }
}
