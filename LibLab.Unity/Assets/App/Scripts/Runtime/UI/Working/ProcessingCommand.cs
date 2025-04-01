// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using VitalRouter;

namespace App.UI.Working
{
    public partial struct ProcessingCommand : ICommand
    {
        public int Completed { get; init; }
        public int Total { get; init; }

        public float Progress => (float)Completed / Total;

        public bool IsFinished => Completed == Total;

        public bool IsStarted => Completed == 0;
    }
}
