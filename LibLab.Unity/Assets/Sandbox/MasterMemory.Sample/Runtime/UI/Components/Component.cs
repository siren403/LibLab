// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEngine;
using UnityEngine.UIElements;
using VitalRouter;

namespace MasterMemory.Sample.UI
{
    [UxmlElement]
    public abstract partial class Component : VisualElement, IBindableRouter
    {
        private CommandOrdering? _commandOrdering;
        private PublishContinuation<DispatchCommand> _dispatchAsyncCallback;
        private Action<DispatchCommand, PublishContext> _dispatchCallback;

        private Subscription _dispatchSubscription;
        private Router _router;

        [UxmlAttribute("self-router")]
        private bool _selfRouter = true;

        protected Component()
        {
            if (!Application.isPlaying) return;
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            RegisterCallback<RegisterDispatchEvent>(OnRegisterDispatchEvent);
        }

        public Router Router
        {
            set
            {
                _router?.UnsubscribeAll();
                _router = value;
                SubscribeDispatch();
            }
            protected get => _router;
        }

        private void OnRegisterDispatchEvent(RegisterDispatchEvent evt)
        {
            evt.StopImmediatePropagation();
            if (evt.target is not DispatchButton button) return;

            string eventName = button.EventName;
            button.clicked += () =>
            {
                _router?.PublishAsync(new DispatchCommand
                {
                    EventName = eventName
                });
            };
        }

        protected void Configure(Action configuration)
        {
            if (!Application.isPlaying) return;
            configuration();
        }

        private void SubscribeDispatch()
        {
            if (_router == null) return;
            if (_dispatchAsyncCallback != null)
            {
                _dispatchSubscription = _router.SubscribeAwait(_dispatchAsyncCallback, _commandOrdering);
            }
            else if (_dispatchCallback != null)
            {
                _dispatchSubscription = _router.Subscribe(_dispatchCallback);
            }
        }

        protected virtual void OnAttachToPanel(AttachToPanelEvent evt)
        {
            if (_selfRouter)
            {
                _router = new Router();
            }
            SubscribeDispatch();
        }

        protected virtual void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            _dispatchSubscription.Dispose();
        }

        protected void Drop(PublishContinuation<DispatchCommand> callback)
        {
            _commandOrdering = CommandOrdering.Drop;
            _dispatchAsyncCallback = callback;
            _dispatchCallback = null;
        }

        protected void Sync(Action<DispatchCommand, PublishContext> callback)
        {
            _dispatchCallback = callback;
            _commandOrdering = null;
            _dispatchAsyncCallback = null;
        }
    }
}
