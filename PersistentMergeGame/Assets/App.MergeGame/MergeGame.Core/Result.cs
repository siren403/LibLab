// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;

namespace MergeGame.Core
{
    public record Result<T>(bool IsSuccess, T Value, string? Error)
    {
        public void Deconstruct(out bool isSuccess, out T value, out string? error)
        {
            isSuccess = IsSuccess;
            value = Value;
            error = Error;
        }
    }

    public record Success<T>(T Value) : Result<T>(true, Value, null);

    public record Failure<T>(string Error) : Result<T>(false, default!, Error)
    {
    }

    public static class ResultExtensions
    {
    }
}
