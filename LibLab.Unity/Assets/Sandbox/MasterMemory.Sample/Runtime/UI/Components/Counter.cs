// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using VitalRouter;

namespace MasterMemory.Sample.UI
{

    public abstract class SubscribeStrategy
    {
        public abstract void Emit(string eventName);

        public class Sync<T> : SubscribeStrategy
        {
            private readonly Dictionary<string, Action<T>> _listeners;

            private readonly Func<T> _targetProvider;

            public Sync(Func<T> targetProvider, int listenerCapacity = 4)
            {
                Assert.IsNotNull(targetProvider);
                _targetProvider = targetProvider;
                _listeners = new Dictionary<string, Action<T>>(listenerCapacity);
            }

            public void Add(string eventName, Action<T> listener)
            {
                _listeners.Add(eventName, listener);
            }

            public override void Emit(string eventName)
            {
                if (_listeners.TryGetValue(eventName, out Action<T> listener))
                {
                    listener(_targetProvider());
                }
            }
        }
    }


    [UxmlElement]
    [Routes]
    public partial class Counter : VisualElement
    {
        private readonly Router _router = new();
        private readonly SubscribeStrategy _subscribeStrategy;

        public Counter()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);

            {
                SubscribeStrategy.Sync<Counter> strategy = new(() => this);
                strategy.Add("increment", static counter => Increment(counter));
                strategy.Add("decrement", static counter => Decrement(counter));
                _subscribeStrategy = strategy;
            }

        }

        [CreateProperty]
        public int Count { get; private set; }

        [Route]
        private void On(DispatchCommand cmd)
        {
            _subscribeStrategy?.Emit(cmd.EventName);
        }

        private void OnClick(ClickEvent evt)
        {
            switch (evt.target)
            {
                case DispatchButton button:
                {
                    evt.StopImmediatePropagation();
                    _router.PublishAsync(new DispatchCommand
                    {
                        EventName = button.EventName
                    });
                    break;
                }
            }
        }

        private static void Increment(Counter counter)
        {
            counter.Count++;
        }

        private static void Decrement(Counter counter)
        {
            counter.Count--;
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            dataSource = this;
            RegisterCallback<ClickEvent>(OnClick);
            MapTo(_router);
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            UnmapRoutes();
            UnregisterCallback<ClickEvent>(OnClick);
            dataSource = null;
        }
    }
}
