// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Supabase;

namespace MergeGame.Infrastructure.Tests
{
    public abstract class SupabaseClientTestBase : IDisposable
    {
        private const string Url = "https://lehzejunsyahkcbdallg.supabase.co";

        private const string Key =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxlaHplanVuc3lhaGtjYmRhbGxnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDgzOTQyNTAsImV4cCI6MjA2Mzk3MDI1MH0.3WwE6Xti6hQ6mZsujLy10f9ZrO1n5rGJGj7QTZV-4VM";

        private const string Schema = "merge_game";

        private Client? _client;

        protected async UniTask<Client> GetClient()
        {
            if (_client != null) return _client;

            var options = new SupabaseOptions { AutoConnectRealtime = true, Schema = Schema };
            var client = new Client(Url, Key, options);
            await client.InitializeAsync().AsUniTask().Timeout(TimeSpan.FromSeconds(5));
            _client = client;

            return _client;
        }

        private readonly CancellationTokenSource _cts = new();
        protected CancellationToken Ct => _cts.Token;

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
