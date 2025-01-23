// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Properties;
using UnityEngine.UIElements;
using VitalRouter;

namespace MasterMemory.Sample.UI
{

    [UxmlElement]
    public partial class Counter : Component
    {
        public Counter()
        {
            Configure(() =>
            {
                dataSource = this;
                // Drop(OnAsync);
                Sync(On);
            });
        }

        [CreateProperty(ReadOnly = true)]
        private int Count { get; set; }

        private void On(DispatchCommand cmd, PublishContext ctx)
        {
            switch (cmd.EventName)
            {
                case "increment":
                    Count = Math.Min(Count + 1, 10);
                    break;
                case "decrement":
                    Count = Math.Max(Count - 1, 0);
                    break;
            }
        }

        private async ValueTask OnAsync(DispatchCommand cmd, PublishContext ctx)
        {
            switch (cmd.EventName)
            {
                case "increment":
                    await Increment(this, ctx.CancellationToken);
                    break;
                case "decrement":
                    await Decrement(this, ctx.CancellationToken);
                    break;
            }
        }

        private static async ValueTask Increment(Counter counter, CancellationToken cancellationToken = default)
        {
            counter.Count++;
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
        }

        private static async ValueTask Decrement(Counter counter, CancellationToken cancellationToken = default)
        {
            counter.Count--;
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
        }
    }
}
