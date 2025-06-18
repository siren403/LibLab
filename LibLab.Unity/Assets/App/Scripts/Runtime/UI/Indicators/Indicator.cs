// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using App.Utils;
using R3;
using SceneLauncher.VContainer;
using UnityEngine;
using VContainer;
using VitalRouter;

namespace App.UI.Indicators
{
    [RequireComponent(typeof(Canvas))]
    public partial class Indicator : ContainerBehaviour
    {
        [SerializeField] private string id;

        private ComponentCache<Canvas> _canvas;
        private ComponentCache<RectTransform> _rectTransform;

        private (bool has, IIndeterminate result) _indeterminate = (false, null);
        private (bool has, IDeterminate result) _determinate = (false, null);

        protected override void Awake()
        {
            base.Awake();
            Canvas canvas = _canvas = gameObject;
            RectTransform tr = _rectTransform = gameObject;

            canvas.enabled = false;
            tr.anchoredPosition = Vector2.zero;

            _indeterminate = (TryGetComponent(out IIndeterminate indeterminate), indeterminate);
            _determinate = (TryGetComponent(out IDeterminate determinate), determinate);
        }

        protected override void OnBuild(IObjectResolver container)
        {
            if (!container.TryResolve(out Router router))
            {
                return;
            }

            if (_indeterminate.has)
            {
                router.SubscribeAwait(async (Working.DelayedCommand command, PublishContext context) =>
                {
                    Canvas canvas = _canvas;
                    canvas.enabled = true;
                    await _indeterminate.result.OnShow();
                }).AddTo(this);
                router.SubscribeAwait(async (Working.EndCommand command, PublishContext context) =>
                {
                    await _indeterminate.result.OnHide();
                    Canvas canvas = _canvas;
                    canvas.enabled = false;
                }).AddTo(this);
            }

            if (_determinate.has)
            {
                router.SubscribeAwait(async (Working.ProcessingCommand command, PublishContext context) =>
                {
                    var indicator = _determinate.result;
                    Canvas canvas = _canvas;
                    switch (command)
                    {
                        case {IsStarted: true}:
                            canvas.enabled = true;
                            await indicator.OnBegin();
                            break;
                        case {IsFinished: true}:
                            await indicator.OnEnd();
                            canvas.enabled = false;
                            break;
                        default:
                            _determinate.result.OnProcessing();
                            break;
                    }
                }).AddTo(this);
            }
        }


        private void Reset()
        {
            gameObject.ResetCanvas();
            gameObject.ResetRectTransform();
        }
    }
}
