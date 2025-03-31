// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEngine;

namespace Storybook
{
    public enum SourceLocation
    {
        Origin,
        Children,
        Manual
    }

    [Serializable]
    public abstract class ComponentSelector
    {
        [SerializeField]
        private SourceLocation location = SourceLocation.Origin;

        public abstract Type SourceType { get; }

        public SourceLocation Location => location;
    }

    [Serializable]
    public class ComponentSelector<T> : ComponentSelector where T : Component
    {
        public override Type SourceType => typeof(T);

        [SerializeField]
        private T source;

        public T Source => source;

        public bool HasSource => source != null;

        public bool TryGetComponent(out T component)
        {
            component = source;
            return HasSource;
        }

        public static implicit operator T(ComponentSelector<T> selector) => selector.Source;
    }
}
