// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.Logging;
using UnityEngine;
using VContainer;
using ZLogger;
using ZLogger.Formatters;

namespace App.Scenes;

public static class ZLoggerExtensions
{
    public static void RegisterZLogger(this IContainerBuilder builder, Action<ILoggingBuilder> configure)
    {
        var loggerFactory = LoggerFactory.Create(configure);
        builder.RegisterInstance(loggerFactory);
        builder.Register(typeof(Logger<>), Lifetime.Singleton).As(typeof(ILogger<>));

        var appLogger = loggerFactory.CreateLogger<Application>();
        builder.RegisterInstance(appLogger);
        // Application.logMessageReceived += onApplicationLog;
        builder.RegisterDisposeCallback(_ =>
        {
            loggerFactory.Dispose();
            // Application.logMessageReceived -= onApplicationLog;
        });

        // void onApplicationLog(string message, string stackTrace, LogType type)
        // {
        //     switch (type)
        //     {
        //         case LogType.Exception:
        //         case LogType.Error:
        //             appLogger.LogCritical(message);
        //             break;
        //     }
        // }
    }

    public static void WithEditorConsolePro(this PlainTextZLoggerFormatter formatter)
    {
        formatter.SetPrefixFormatter($"{0} | {1:short} | {2} | ",
            (in MessageTemplate template, in LogInfo info) =>
                template.Format(info.Timestamp, info.LogLevel, info.Category));
    }
}
