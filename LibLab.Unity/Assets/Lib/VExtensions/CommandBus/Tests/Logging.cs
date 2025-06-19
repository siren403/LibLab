// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using VitalRouter;

namespace VExtensions.CommandBus.Tests
{
    public class Logging : ICommandInterceptor
    {
        public async ValueTask InvokeAsync<T>(T command, PublishContext context, PublishContinuation<T> next)
            where T : ICommand
        {
            UnityEngine.Debug.Log($"Start {typeof(T)}");
            // Execute subsequent routes.
            await next(command, context);
            UnityEngine.Debug.Log($"End   {typeof(T)}");
        }
    }
}
