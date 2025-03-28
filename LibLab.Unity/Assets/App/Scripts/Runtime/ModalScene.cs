// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using R3;
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
            builder.Register<LogicService>(Lifetime.Singleton).AsSelf();

            builder.Register<AlertState>(Lifetime.Scoped).AsSelf();
            builder.Register<AlertLogic>(Lifetime.Scoped).As<ILogic>();
            builder.Register<AlertFailed>(Lifetime.Scoped).As<ILogic>();
        }
    }

    public class AlertState
    {
        public IReadOnlyBindableReactiveProperty<string> Message => _message;

        private readonly BindableReactiveProperty<string> _message = new();

        public void Success()
        {
            _message.Value = "Success";
        }

        public void Fail()
        {
            _message.Value = "Fail";
        }
    }

    public class AlertLogic : ILogic
    {
        private readonly AlertState _state;
        public string Id => "alert";

        private int _count;

        public AlertLogic(AlertState state)
        {
            _state = state;
        }

        public LogicResult Process(CancellationToken cancellationToken)
        {
            _count++;
            if (_count <= 3)
            {
                _state.Success();
                return LogicResult.Success;
            }

            _state.Fail();
            return LogicResult.Failed;
        }
    }

    public class AlertFailed : ILogic
    {
        public string Id => "alert:fail";

        public LogicResult Process(CancellationToken cancellationToken)
        {
            return LogicResult.Success;
        }
    }

    public class PageContainer : Dictionary<string, Page>
    {
    }
}
