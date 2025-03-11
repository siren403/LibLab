// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;

namespace Storybook
{
    public enum CacheLocation
    {
        Origin,
        Children
    }

    public sealed class ComponentCache<T> where T : Component
    {
        private T _component;
        private bool _hasComponent;
        private CacheLocation _location;

        public ComponentCache(CacheLocation location = CacheLocation.Origin)
        {
            _location = location;
        }

        public T Get(GameObject target)
        {
            if (_hasComponent)
            {
                return _component;
            }

            switch (_location)
            {
                case CacheLocation.Origin:
                    _hasComponent = target.TryGetComponent(out _component);
                    break;
                case CacheLocation.Children:
                    _component = target.GetComponentInChildren<T>();
                    _hasComponent = _component != null;
                    break;
            }
            if (!_hasComponent)
            {
                Debug.LogWarning($"Component {typeof(T).Name} is not attached to target {target}");
            }
            return _component;
        }
    }
}
