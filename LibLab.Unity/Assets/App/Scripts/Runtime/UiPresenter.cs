// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VitalRouter;

namespace App
{
    public partial struct ClickCommand : ICommand
    {
        public string Id { get; init; }
    }

    public enum LogicResult
    {
        Success,
        Failed,
    }

    public interface ILogic
    {
        string Id { get; }
        LogicResult Process(CancellationToken cancellationToken);
    }

    public class LogicService
    {
        private readonly Dictionary<string, ILogic> _logics;

        public LogicService(IReadOnlyList<ILogic> logics)
        {
            _logics = logics.ToDictionary((logic) => logic.Id, logic => logic);
        }

        public async UniTask<LogicResult> To(string id, CancellationToken cancellationToken)
        {
            return _logics.TryGetValue(id, out var logic) ? logic.Process(cancellationToken) : LogicResult.Success;
        }
    }

    public class ClickInterceptor : ICommandInterceptor
    {
        private readonly LogicService _logics;

        public ClickInterceptor(LogicService logics)
        {
            _logics = logics;
        }

        public async ValueTask InvokeAsync<T>(T command, PublishContext context, PublishContinuation<T> next)
            where T : ICommand
        {
            if (command is ClickCommand click)
            {
                var result = await _logics.To(click.Id, context.CancellationToken);
                Debug.Log($"{click.Id}: {result}");
                if (result == LogicResult.Failed)
                {
                    return;
                }
            }

            await next(command, context);
        }
    }

    [Routes]
    public partial class UiPresenter
    {
        private readonly PageContainer _pages;

        public UiPresenter(PageContainer pages)
        {
            _pages = pages;
        }

        [Route(CommandOrdering.Drop)]
        [Filter(typeof(ClickInterceptor))]
        private async ValueTask OnClick(ClickCommand command, CancellationToken cancellationToken)
        {
            Debug.Log("Clicked: " + command.Id);
            if (_pages.TryGetValue(command.Id, out var page))
            {
                await page.Show();
            }
        }
    }
}
