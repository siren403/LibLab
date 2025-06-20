// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VExtensions.CommandBus.Tests
{
    public class SetFullNameHandler : ICommandHandler<SetFullName>
    {
        public UniTask ExecuteAsync(SetFullName command, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(command.FirstName) || string.IsNullOrEmpty(command.LastName))
            {
                throw new ArgumentException("First name and last name cannot be empty.");
            }

            return UniTask.CompletedTask;
        }
    }
}
