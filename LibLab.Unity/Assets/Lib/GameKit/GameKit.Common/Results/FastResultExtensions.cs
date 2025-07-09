// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameKit.Common.Results
{
    public static class FastResultExtensions
    {
        public static FastResult<TCast> Pass<T, TCast>(this FastResult<T> result)
        {
            if (!result.IsError)
            {
                Debug.LogWarning("Cannot convert a successful result to a failure result.");
            }

            return result.Cast<TCast>();
        }

        public static bool IsError<T, TCast>(this FastResult<T> result, out FastResult<TCast> fail)
        {
            if (result.IsError)
            {
                fail = result.Pass<T, TCast>();
                return true;
            }

            fail = default;
            return false;
        }
    }
}
