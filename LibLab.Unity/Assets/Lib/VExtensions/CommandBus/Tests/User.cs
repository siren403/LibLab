// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using Cysharp.Threading.Tasks;

namespace VExtensions.CommandBus.Tests
{
    public class User
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class SetUserFirstName : ICommandWithoutResult
    {
        public string Name { get; init; } = string.Empty;
    }

    public class GetUserFirstName : ICommand<string>
    {
    }

    public class GetUserFirstNameHandler : ICommandHandler<GetUserFirstName, string>
    {
        private readonly User _user;

        public GetUserFirstNameHandler(User user)
        {
            _user = user;
        }

        public UniTask<string> ExecuteAsync(GetUserFirstName command, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(_user.FirstName))
            {
                throw new System.InvalidOperationException("First name is not set.");
            }

            return UniTask.FromResult(_user.FirstName);
        }
    }

    public class SetUserFirstNameHandler : ICommandHandler<SetUserFirstName>
    {
        private readonly User _user;

        public SetUserFirstNameHandler(User user)
        {
            _user = user;
        }

        public UniTask ExecuteAsync(SetUserFirstName command, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(command.Name))
            {
                throw new System.ArgumentException("Name cannot be empty.");
            }

            _user.FirstName = command.Name;
            return UniTask.CompletedTask;
        }
    }
}
