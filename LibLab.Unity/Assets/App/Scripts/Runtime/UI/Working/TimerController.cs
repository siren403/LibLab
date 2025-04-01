// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ImprovedTimers;
using UnityEngine;
using VitalRouter;

namespace App.UI.Working
{
    [Routes]
    public partial class TimerController : IDisposable
    {
        private readonly Router _router;
        private readonly WorkingConfig _config;
        private readonly CountdownTimer _timer = new(1);
        private TimeSpan _timerDuration;

        public TimerController(Router router, WorkingConfig config)
        {
            _router = router;
            _config = config;
            _timer.OnTimerStop += OnStoppedTimer;
        }

        private void OnStoppedTimer()
        {
            _router.PublishAsync(new DelayedCommand() { Duration = _timerDuration });
        }

        [Route(CommandOrdering.Switch)]
        private async ValueTask OnBegin(BeginCommand command, CancellationToken cancellationToken)
        {
            _timer.Stop();
            _timerDuration = _config.ExpectDuration;
            _timer.Reset((float)_timerDuration.TotalSeconds);
            _timer.Start();
            cancellationToken.Register(() => { _timer.Pause(); });
            await UniTask.Delay(_timerDuration, cancellationToken: cancellationToken);
        }

        [Route]
        private void OnEnd(EndCommand command)
        {
            _timer.Pause();
            _timer.Reset();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
