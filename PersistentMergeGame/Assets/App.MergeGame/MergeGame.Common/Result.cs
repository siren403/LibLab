// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine.Assertions;

namespace MergeGame.Common
{
    public record Result(int StatusCode)
    {
        public bool IsOk => StatusCode == 0;

        public static Ok Ok()
        {
            return new Ok();
        }

        public static Error Error(string message, int code = -1)
        {
            Assert.IsTrue(code != 0, "Error code must be greater than zero.");
            return new Error(code, message);
        }
    }

    public record Ok() : Result(0);

    public record Error(int StatusCode, string Message) : Result(StatusCode);


    public record Result<T>(int StatusCode) : Result(StatusCode)
    {
        public static Ok<T> Ok(T data)
        {
            return new Ok<T>(data);
        }

        public static new Error<T> Error(string message, int code = -1)
        {
            Assert.IsTrue(code != 0, "Error code must be greater than zero.");
            return new Error<T>(code, message);
        }
    }

    public record Ok<T>(T Data) : Result<T>(0);

    public record Error<T>(int StatusCode, string Message) : Result<T>(StatusCode);
}
