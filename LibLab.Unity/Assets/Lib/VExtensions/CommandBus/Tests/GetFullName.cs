// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace VExtensions.CommandBus.Tests
{
    public class GetFullName : ICommand<string>
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
}
