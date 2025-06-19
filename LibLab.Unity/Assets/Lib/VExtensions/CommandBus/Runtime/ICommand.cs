// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace VExtensions.CommandBus
{
    public sealed class Void
    {
        internal static readonly Void _instance = new();
    }

    // public interface ICommandBase
    // {
    // }

    public interface ICommandBus : ICommand<Void>
    {
    }

    public interface ICommand<out TResult> : VitalRouter.ICommand
    {
    }
}
