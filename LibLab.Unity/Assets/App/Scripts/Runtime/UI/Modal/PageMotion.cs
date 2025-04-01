// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using App.UI.Pages;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Animation;
using UnityEngine;

namespace App.UI.Modal
{
    public class PageMotion : MonoBehaviour, IPageAnimation
    {
        [SerializeField] private LitMotionAnimation show;
        [SerializeField] private LitMotionAnimation hide;
        [SerializeField] private float timeoutSeconds = 1f;
        [SerializeField] private bool cancelOnComplete = false;

        private LitMotionAnimation _playing;

        public async UniTask Show(CancellationToken cancellationToken = default)
        {
            _playing = show;
            show.Play();
            await UniTask.WaitWhile(show, static s => s.IsPlaying, cancellationToken: cancellationToken)
                .Timeout(TimeSpan.FromSeconds(timeoutSeconds));
            show.Stop();
            _playing = null;
        }

        public async UniTask Hide(CancellationToken cancellationToken = default)
        {
            _playing = hide;
            hide.Play();
            await UniTask.WaitWhile(hide, static s => s.IsPlaying, cancellationToken: cancellationToken)
                .Timeout(TimeSpan.FromSeconds(timeoutSeconds));
            hide.Stop();
            _playing = null;
        }

        public void Cancel()
        {
            if (_playing == null)
            {
                return;
            }

            if (cancelOnComplete)
            {
                _playing.Stop();
                foreach (LitMotionAnimationComponent c in _playing.Components)
                {
                    c.TrackedHandle.TryComplete();
                }
            }
            _playing = null;
        }
    }
}
