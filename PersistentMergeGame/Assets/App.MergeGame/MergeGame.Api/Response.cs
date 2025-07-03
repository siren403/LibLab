// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace MergeGame.Api
{
    public record Response(int StatusCode)
    {
        public bool IsOk => StatusCode == 0;
    }
}
