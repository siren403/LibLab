// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;

namespace VExtensions.CommandBus.Tests
{
    public abstract class TestWithCancellationToken : IDisposable
    {
        private readonly CancellationTokenSource _disposeCancellationTokenSource = new();
        protected CancellationToken DisposeCancellationToken => _disposeCancellationTokenSource.Token;

        public void Dispose()
        {
            _disposeCancellationTokenSource.Cancel();
            _disposeCancellationTokenSource.Dispose();
            OnDisposed();
        }

        protected virtual void OnDisposed()
        {
            throw new NotImplementedException("Override this method to handle disposal logic.");
        }
    }
}
