// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using VContainer;
using VitalRouter;
using VitalRouter.VContainer;

namespace App.UI.Working
{
    public static class WorkingExtensions
    {
        public static WorkHandle ReadyWork(this Router router)
        {
            return new WorkHandle(router);
        }

        public static void RegisterWorking(this IContainerBuilder builder, Action<WorkingBuilder> configure)
        {
            var working = new WorkingBuilder(builder);
            configure(working);
            builder.RegisterVitalRouter(routing => { routing.Map<TimerController>(); });
            builder.RegisterInstance(new WorkingConfig(
                ExpectDuration: working.ExpectDuration
            ));
        }
    }

    public class WorkingBuilder
    {
        private readonly IContainerBuilder _builder;
        public TimeSpan ExpectDuration { get; set; }

        public WorkingBuilder(IContainerBuilder builder)
        {
            _builder = builder;
        }
    }

    public struct WorkHandle : IDisposable
    {
        private readonly Router _router;

        public WorkHandle(Router router)
        {
            _router = router;
        }

        public void Begin()
        {
            _router.PublishAsync(new BeginCommand());
        }

        public void Processing(int completed, int total)
        {
            _router.PublishAsync(new ProcessingCommand() { Completed = completed, Total = total });
        }

        public void Dispose()
        {
            _router.PublishAsync(new EndCommand());
        }
    }
}
