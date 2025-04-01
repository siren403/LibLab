// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using R3;

namespace App.Scenes.Modal
{
    public class AlertState
    {
        public IReadOnlyBindableReactiveProperty<string> Message => _message;

        private readonly BindableReactiveProperty<string> _message = new("Pending");

        public void Success()
        {
            _message.Value = "Success";
        }

        public void Fail()
        {
            _message.Value = "Fail";
        }
    }
}
