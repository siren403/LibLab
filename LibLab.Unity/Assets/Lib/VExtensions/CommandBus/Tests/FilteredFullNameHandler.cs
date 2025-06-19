// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;
using VitalRouter;

namespace VExtensions.CommandBus.Tests
{
    [Routes]
    [Filter(typeof(Logging))]
    public partial class FilteredFullNameHandler : CommandHandler<GetFullName, string>
    {
        public override UniTask<string> ExecuteAsync(GetFullName command, CancellationToken ct)
        {
            return UniTask.FromResult($"{command.FirstName} {command.LastName} (filtered)");
        }
    }
}
