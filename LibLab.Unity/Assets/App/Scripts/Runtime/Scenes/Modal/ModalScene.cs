// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using App.Services;
using App.UI.Pages;
using VContainer;
using VContainer.Unity;

namespace App.Scenes.Modal
{
    public class ModalScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterPages();
            // builder.RegisterVitalRouter((routing) => { routing.Map<UiPresenter>(); });
            // builder.Register<LogicService>(Lifetime.Singleton).AsSelf();
            //
            builder.Register<AlertState>(Lifetime.Singleton).AsSelf();
            // builder.Register<AlertLogic>(Lifetime.Scoped).As<ILogic>();
            // builder.Register<AlertFailed>(Lifetime.Scoped).As<ILogic>();
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

            return LogicResult.Failed;
        }
    }

    public class AlertFailed : ILogic
    {
        public string Id => "alert:fail";
        private readonly AlertState _state;

        public AlertFailed(AlertState state)
        {
            _state = state;
        }

        public LogicResult Process(CancellationToken cancellationToken)
        {
            _state.Fail();
            return LogicResult.Success;
        }
    }
}
