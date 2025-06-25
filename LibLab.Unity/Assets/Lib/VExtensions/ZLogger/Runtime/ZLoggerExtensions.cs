// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.Logging;
using UnityEngine;
using VContainer;
using ZLogger;
using ZLogger.Formatters;

namespace VExtensions.ZLogger
{
    public static class ZLoggerExtensions
    {
        public static void RegisterZLogger(this IContainerBuilder builder, Action<ILoggingBuilder> configure)
        {
            if (builder.Exists(typeof(ILoggerFactory), includeInterfaceTypes: true, findParentScopes: true)) return;

            var loggerFactory = LoggerFactory.Create(configure);
            builder.RegisterInstance(loggerFactory);
            builder.Register(typeof(Logger<>), Lifetime.Singleton).As(typeof(ILogger<>));

            var appLogger = loggerFactory.CreateLogger<Application>();
            builder.RegisterInstance(appLogger);
            builder.RegisterDisposeCallback(_ => { loggerFactory.Dispose(); });
        }

        public static void WithEditorConsolePro(this PlainTextZLoggerFormatter formatter)
        {
            formatter.SetPrefixFormatter($"{0} | {1:short} | ({2}) | ",
                (in MessageTemplate template, in LogInfo info) =>
                    template.Format(info.Timestamp, info.LogLevel, info.Category));
        }
    }
}
