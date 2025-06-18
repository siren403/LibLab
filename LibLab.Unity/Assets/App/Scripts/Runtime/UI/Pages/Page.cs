// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using App.Utils;
using Cysharp.Threading.Tasks;
using SceneLauncher.VContainer;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace App.UI.Pages
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class Page : MonoBehaviour, IPage
    {
        [SerializeField] private string id = null!;

        private ComponentCache<Canvas> _canvas;
        private ComponentCache<GraphicRaycaster> _raycaster;
        private ComponentCache<RectTransform> _rectTransform;

        private ContainerProvider? _container;
        private (bool has, IPageAnimation? result) _animation = (false, null);

        private void Awake()
        {
            Canvas canvas = _canvas = gameObject;
            _raycaster = gameObject;
            RectTransform tr = _rectTransform = gameObject;

            _animation.has = TryGetComponent(out _animation.result);

            canvas.enabled = false;
            tr.anchoredPosition = Vector2.zero;

            _container = new ContainerProvider(gameObject);
            _container.GetContainer().ContinueWith(container =>
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    Debug.LogWarning($"${gameObject.name} is empty page id");
                    return;
                }

                PageNavigator navigator = container.Resolve<PageNavigator>();
                navigator.Add(id, this);
            });
        }

        public async UniTask Show(CancellationToken cancellationToken = default)
        {
            Canvas canvas = _canvas;
            canvas.enabled = true;
            try
            {
                if (_animation.has)
                {
                    await _animation.result!.Show(cancellationToken);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during show animation: {e}");
                if (_animation.has)
                {
                    _animation.result!.Cancel();
                }
            }
        }

        public async UniTask Hide(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_animation.has)
                {
                    await _animation.result!.Hide(cancellationToken);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during hide animation: {e}");
                if (_animation.has)
                {
                    _animation.result!.Cancel();
                }
            }
            finally
            {
                Canvas canvas = _canvas;
                canvas.enabled = false;
            }
        }

        private void Reset()
        {
            gameObject.ResetCanvas();
            gameObject.ResetRectTransform();
        }
    }
}
