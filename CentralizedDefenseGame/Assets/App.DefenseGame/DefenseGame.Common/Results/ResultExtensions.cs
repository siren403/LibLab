// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace DefenseGame.Common.Results
{
    public static class ResultExtensions
    {
        public static Fail<T> Pass<T>(this Result result)
        {
            if (!result.IsError)
            {
                throw new System.InvalidOperationException(
                    "Cannot convert a successful result to a failure result.");
            }

            var errors = result.GetErrors();
            return Result<T>.Fail(errors);
        }

        public static bool IsErrorWithPass<T>(this Result result, out Fail<T> fail)
        {
            if (result.IsError)
            {
                fail = result.Pass<T>();
                return true;
            }

            fail = null!;
            return false;
        }
    }
}
