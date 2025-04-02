// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using App.UI.Indicators;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Animation;
using UnityEngine;

namespace App.Scenes.Indicators
{
    public class Spinner : MonoBehaviour, IIndeterminate
    {
        [SerializeField] private LitMotionAnimation show;
        [SerializeField] private LitMotionAnimation loop;
        [SerializeField] private LitMotionAnimation hide;

        public UniTask OnShow()
        {
            show.Play();
            loop.Play();
            return UniTask.CompletedTask;
        }

        public async UniTask OnHide()
        {
            hide.Play();
            await UniTask.WaitWhile(() => hide.IsPlaying);
            hide.Stop();
            loop.Stop();
            show.Stop();
        }
    }
}
