// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.Logging;
using UnityEngine;
using Utf8StringInterpolation;
using VContainer;
using VContainer.Unity;
using ZLogger;
using ZLogger.Formatters;
using ZLogger.Unity;

namespace App.Scenes
{
    internal class MainScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<AddressableRouterEntry>();
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
        }
    }

    public static class ZLoggerExtensions
    {
        public static void RegisterZLogger(this IContainerBuilder builder, Action<ILoggingBuilder> configure)
        {
            var loggerFactory = LoggerFactory.Create(configure);
            builder.RegisterInstance(loggerFactory);
            builder.Register(typeof(Logger<>), Lifetime.Singleton).As(typeof(ILogger<>));

            var appLogger = loggerFactory.CreateLogger<Application>();
            builder.RegisterInstance(appLogger);
            Application.logMessageReceived += onApplicationLog;
            builder.RegisterDisposeCallback(_ =>
            {
                loggerFactory.Dispose();
                Application.logMessageReceived -= onApplicationLog;
            });

            void onApplicationLog(string message, string stackTrace, LogType type)
            {
                switch (type)
                {
                    case LogType.Exception:
                        appLogger.LogCritical(message);
                        break;
                }
            }
        }

        public static void WithEditorConsolePro(this PlainTextZLoggerFormatter formatter)
        {
            formatter.SetPrefixFormatter($"{0} | {1:short} | {2} | ",
                (in MessageTemplate template, in LogInfo info) =>
                    template.Format(info.Timestamp, info.LogLevel, info.Category));
        }
    }
}
