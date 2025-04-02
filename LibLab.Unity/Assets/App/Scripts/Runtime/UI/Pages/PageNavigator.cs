// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.Assertions;
using ZLogger;

namespace App.UI.Pages
{
    public class PageNavigator
    {
        private readonly ILogger<PageNavigator> _logger;
        private readonly Dictionary<string, IPage> _pages = new();
        private readonly Stack<string> _history = new();
        private bool _processing;

        public PageNavigator(ILogger<PageNavigator> logger)
        {
            _logger = logger;
        }

        public void Add(string id, IPage page)
        {
            _pages.Add(id, page);
            _logger.AddedPage(id);
        }

        public async UniTask Push(string id, CancellationToken cancellationToken = default)
        {
            Assert.IsNotNull(id);
            if (_processing)
            {
                return;
            }

            if (!_pages.TryGetValue(id, out IPage page))
            {
                _logger.NotFoundPage(id, "Push");
                return;
            }

            if (_history.Count > 0 && _history.Contains(id))
            {
                _logger.ContainsPage(id);
                return;
            }

            _history.Push(id);
            try
            {
                _logger.BeginShowPage(id);
                _processing = true;
                await page.Show(cancellationToken);
            }
            catch (Exception e)
            {
                string errorId = _history.Pop();
                _logger.ShowFailedPage(errorId, e.Message);
            }
            finally
            {
                _processing = false;
            }
        }

        public async UniTask Pop(CancellationToken cancellationToken = default)
        {
            if (_processing)
            {
                return;
            }

            if (_history.Count == 0)
            {
                _logger.EmptyHistory();
                return;
            }

            string id = _history.Pop();
            if (!_pages.TryGetValue(id, out IPage page))
            {
                _logger.NotFoundPage(id, "Pop");
                return;
            }

            try
            {
                _logger.BeginHidePage(id);
                _processing = true;
                await page.Hide(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.HideFailedPage(id, e.Message);
            }
            finally
            {
                _processing = false;
            }
        }
    }
}
