// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace DefenseGame.Common.Results
{
    public readonly struct Error
    {
        public string Code { get; init; }

        public static implicit operator Error(string code) => new() { Code = code };
    }
}
