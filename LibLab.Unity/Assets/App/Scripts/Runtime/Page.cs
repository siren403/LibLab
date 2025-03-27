// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using SceneLauncher.VContainer;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace App
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class Page : MonoBehaviour
    {
        [SerializeField] private string id;

        private Canvas _canvas;
        private RectTransform _transform;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _transform = GetComponent<RectTransform>();

            _canvas.enabled = false;
            _transform.anchoredPosition = Vector2.zero;

            PostLaunchLifetimeScope.GetLaunchedTask(gameObject.scene).ContinueWith((container) =>
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    Debug.LogWarning($"${gameObject.name} is empty page id");
                    return;
                }

                var pages = container.Resolve<PageContainer>();
                pages.Add(id, this);
            });
        }

        public async UniTask Show()
        {
            _canvas.enabled = true;
            var scrim = transform.Find("Scrim").GetComponent<CanvasGroup>();
            LMotion.Create(0f, 1f, 0.3f)
                .BindToAlpha(scrim)
                .ToUniTask(destroyCancellationToken);

            var content = transform.Find("Content");
            await LMotion.Create(Vector2.zero, Vector2.one, 0.3f)
                .WithEase(Ease.OutExpo)
                .BindToLocalScaleXY(content)
                .ToUniTask(destroyCancellationToken);
        }

        public async UniTask Hide()
        {
            var scrim = transform.Find("Scrim").GetComponent<CanvasGroup>();
            LMotion.Create(1f, 0f, 0.3f)
                .BindToAlpha(scrim)
                .ToUniTask(destroyCancellationToken);

            var content = transform.Find("Content");
            await LMotion.Create(Vector2.one, Vector2.zero, 0.3f)
                .WithEase(Ease.OutExpo)
                .BindToLocalScaleXY(content)
                .ToUniTask(destroyCancellationToken);
            _canvas.enabled = false;
        }

        private void Reset()
        {
            var canvas = GetComponent<Canvas>();
            canvas.vertexColorAlwaysGammaSpace = true;

            var tr = GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;

            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;
        }
    }
}
