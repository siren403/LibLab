// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
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

    [Routes]
    public partial class UiPresenter
    {
        [Route(CommandOrdering.Drop)]
        private async ValueTask OnClick(ClickCommand command, CancellationToken cancellationToken)
        {
            Debug.Log("Clicked: " + command.Id);
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
        }
    }
}
