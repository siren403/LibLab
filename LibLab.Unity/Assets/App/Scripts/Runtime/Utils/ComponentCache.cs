// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;
using UnityEngine.Assertions;

namespace App.Utils
{
    public readonly struct ComponentCache<T> where T : Component
    {
        private readonly T _component;

        private ComponentCache(GameObject source)
        {
            Assert.IsNotNull(source);
            _component = source.GetComponent<T>();
            Assert.IsNotNull(_component);
        }

        public T Get()
        {
            return _component;
        }

        public static implicit operator ComponentCache<T>(GameObject source) => new(source);
        public static implicit operator T(ComponentCache<T> cache) => cache.Get();
    }
}
