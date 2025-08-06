// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using VContainer;
using VContainer.Unity;
using SceneNavigation.Extensions;
using VExtensions.ZLogger;
using VitalRouter.VContainer;
using ZLogger;
using ZLogger.Unity;

namespace App.Scenes
{
    internal class MainScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterZLogger(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
// #if UNITY_EDITOR
                logging.AddZLoggerRollingFile((dt, index) => $"Logs/{dt:yyyy-MM-dd}_{index}.log", 1024 * 1024);
// #endif
                logging.AddZLoggerUnityDebug(options =>
                {
                    options.UsePlainTextFormatter(formatter => { formatter.WithEditorConsolePro(); });
                });
            });

            builder.RegisterVitalRouter((routing) => { });
            builder.RegisterNavigator((nav) =>
            {
                nav.StartupRootOnlyMainScene("/intro");
            });


        }
    }
}
