// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace App.UI.Pages
{
    public static partial class PageLogger
    {
        [ZLoggerMessage(LogLevel.Information, "AddPage: {id}")]
        public static partial void AddedPage(this ILogger<PageNavigator> logger, string id);

        [ZLoggerMessage(LogLevel.Error, "Page not found: {id} | {reason}")]
        public static partial void NotFoundPage(this ILogger<PageNavigator> logger, string id, string reason);

        [ZLoggerMessage(LogLevel.Warning, "Already pushed page: {id}")]
        public static partial void ContainsPage(this ILogger<PageNavigator> logger, string id);

        [ZLoggerMessage(LogLevel.Information, "Pushing page: {id}")]
        public static partial void BeginShowPage(this ILogger<PageNavigator> logger, string id);

        [ZLoggerMessage(LogLevel.Error, "Failed to add page: {id} {ex}")]
        public static partial void ShowFailedPage(this ILogger<PageNavigator> logger, string id, string ex);

        [ZLoggerMessage(LogLevel.Warning, "Empty history")]
        public static partial void EmptyHistory(this ILogger<PageNavigator> logger);

        [ZLoggerMessage(LogLevel.Information, "Popping page: {id}")]
        public static partial void BeginHidePage(this ILogger<PageNavigator> logger, string id);

        [ZLoggerMessage(LogLevel.Error, "Failed to pop page: {id} {ex}")]
        public static partial void HideFailedPage(this ILogger<PageNavigator> logger, string id, string ex);
    }
}
