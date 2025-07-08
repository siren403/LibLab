// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace GameKit.Common.Results
{
    public readonly struct Error
    {
        public string Code { get; init; }
        public string Description { get; init; }

        public static implicit operator Error(string code) => new() { Code = code, Description = string.Empty };

        public override string ToString()
        {
            return string.IsNullOrEmpty(Description)
                ? $"Error: {Code}"
                : $"Error: {Code}, Description: {Description}";
        }
    }
}
