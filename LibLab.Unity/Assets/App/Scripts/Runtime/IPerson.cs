// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace App
{
    internal interface IPerson
    {
        string Name { get; }
    }

    internal class MainPerson : IPerson
    {
        public string Name => "Main";
    }

    internal class AlertPerson : IPerson
    {
        public string Name => "Alert";
    }

}
