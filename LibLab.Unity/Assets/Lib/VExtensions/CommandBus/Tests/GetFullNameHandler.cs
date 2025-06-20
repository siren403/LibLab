// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;

namespace VExtensions.CommandBus.Tests
{
    public class GetFullNameHandler : ICommandHandler<GetFullName, string>
    {
        public UniTask<string> ExecuteAsync(GetFullName command, CancellationToken ct)
        {
            return UniTask.FromResult($"{command.FirstName} {command.LastName}");
        }
    }
}
