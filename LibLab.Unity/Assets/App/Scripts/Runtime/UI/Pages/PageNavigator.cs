// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace App.UI.Pages
{
    public class PageNavigator
    {
        private readonly Dictionary<string, IPage> _pages = new();
        private readonly Stack<string> _history = new();
        private bool _processing;

        public void Add(string id, IPage page)
        {
            _pages.Add(id, page);
            Debug.Log($"Added Page {id}");
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
                return;
            }

            if (_history.Count > 0 && _history.Peek() == id)
            {
                Debug.LogWarning($"Already pushing page {id}");
                return;
            }

            _history.Push(id);
            try
            {
                Debug.Log($"Pushing page {id}");
                _processing = true;
                await page.Show(cancellationToken);
            }
            catch
            {
                string errorId = _history.Pop();
                Debug.LogError("Error showing page: " + errorId);
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
                Debug.LogWarning("No pages to pop");
                return;
            }

            string id = _history.Pop();
            if (!_pages.TryGetValue(id, out IPage page))
            {
                Debug.LogWarning($"Page with id {id} not found");
                return;
            }

            try
            {
                Debug.Log($"Popping page {id}");
                _processing = true;
                await page.Hide(cancellationToken);
            }
            catch
            {
                Debug.LogError("Error hiding page: " + id);
            }
            finally
            {
                _processing = false;
            }
        }
    }
}
