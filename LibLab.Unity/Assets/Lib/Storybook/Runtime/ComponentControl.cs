// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEngine;

namespace Storybook
{
    [Serializable]
    public abstract class ComponentControl<TSource, TValue> : IControl where TSource : Component
    {
        [SerializeField]
        private ComponentSelector<TSource> selector;

        [SerializeField]
        private TValue value;

        public Type SourceType => selector.SourceType;
        public Type ValueType => typeof(TValue);

        public Component Source => selector.Source;

        public TValue Value
        {
            get => value;
            set => this.value = value;
        }

        public ChangeResult OnValueChanged()
        {
            if (!selector.HasSource)
            {
                Debug.LogWarning($"Control.OnValueChanged({selector}, {value}): source is null");
                return ChangeResult.Failure;
            }
            return OnValueChanged(selector.Source, value);
        }

        public ComponentControl()
        {

        }

        public ComponentControl(TValue initialValue)
        {
            value = initialValue;
        }

        protected abstract ChangeResult OnValueChanged(TSource source, TValue value);

        public override string ToString()
        {
            return $"{value}";
        }
    }
}
