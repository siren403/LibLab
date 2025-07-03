// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;

namespace MergeGame.Core
{
    public record Result<T>(bool IsSuccess, T Value, string? Message)
    {
        public void Deconstruct(out bool isSuccess, out T value, out string? error)
        {
            isSuccess = IsSuccess;
            value = Value;
            error = Message;
        }
    }

    public record Ok<T>(T Value) : Result<T>(true, Value, null);

    public record Error<T>(string Message) : Result<T>(false, default!, Message)
    {
    }

    public static class ResultExtensions
    {
    }
}
