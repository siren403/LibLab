// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using UnityEngine;
using VitalRouter;

namespace App.UI.Pages
{
    [Routes]
    public partial class PageController
    {
        private readonly PageNavigator _navigator;

        public PageController(PageNavigator navigator)
        {
            _navigator = navigator;
        }

        [Route(CommandOrdering.Drop)]
        private async ValueTask OnPush(PushCommand command, PublishContext context)
        {
            await _navigator.Push(command.Id, context.CancellationToken);
        }

        [Route(CommandOrdering.Drop)]
        private async ValueTask OnPop(PopCommand command, PublishContext context)
        {
            await _navigator.Pop(context.CancellationToken);
        }
    }
}
