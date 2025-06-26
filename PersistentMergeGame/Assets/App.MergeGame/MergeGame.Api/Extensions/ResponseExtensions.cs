// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace MergeGame.Api.Extensions
{
    public static class ResponseExtensions
    {
        public static bool Ok(this Response response) => response.StatusCode == 0;
        public static bool Error(this Response response) => response.StatusCode != 0;
    }
}
