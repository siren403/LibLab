// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Properties;
using UnityEngine.UIElements;
using VitalRouter;
using VitalRouter.MRuby;

namespace MasterMemory.Sample.UI
{
    [MRubyObject]
    public partial struct AddCommand : ICommand
    {
        public const string Key = "add";
    }

    public partial class CalculatorPresenter
    {
        public int a;
        public int b;

        public bool isBusy;
        public int result = 1111;

        public async ValueTask On(CancellationToken cancellationToken)
        {
            isBusy = true;
            int total = a + b;
            result = 0;
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            result = total;
            isBusy = false;
        }
    }

    [UxmlObject]
    public partial class Coin
    {
        [UxmlAttribute]
        public int Value;
    }

    [UxmlElement]
    public partial class Calculator : RouterElement
    {
        [CreateProperty]
        public SampleObject SampleObject { get; set; }
    }
}
