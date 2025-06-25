// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.Logging;
using VContainer;
using VExtensions.ZLogger;
using VitalRouter.VContainer;
using ZLogger.Unity;

namespace MergeGame.Api.Tests
{
    public static class TestUtils
    {
        public static IObjectResolver BuildApi(Action<IContainerBuilder>? configuration = null)
        {
            var bld = new ContainerBuilder();
            bld.RegisterZLogger(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddZLoggerUnityDebug(options =>
                {
                    options.UsePlainTextFormatter(formatter => { formatter.WithEditorConsolePro(); });
                });
            });
            bld.RegisterVitalRouter(routing => { });
            bld.RegisterMergeGame();
            configuration?.Invoke(bld);
            return bld.Build();
        }
    }
}
