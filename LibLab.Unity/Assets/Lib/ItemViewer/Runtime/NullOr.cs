// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEngine;

namespace ItemViewer
{
    [Serializable]
    public struct NullOr<T>
    {
        public bool IsNull
        {
            get
            {
                _isNull ??= value is null;
                return _isNull.Value;
            }
        }

        private bool? _isNull;
        [SerializeField] private T? value;

        public T Value
        {
            get => value!;
        }

        public NullOr(T? value)
        {
            this.value = value;
            _isNull = this.value is null;
        }

        public static implicit operator NullOr<T>(T value)
        {
            return new NullOr<T>(value);
        }

        public static implicit operator bool(NullOr<T> value)
        {
            return !value.IsNull;
        }

        public static implicit operator T(NullOr<T> value)
        {
            return value.Value;
        }

        public override string ToString()
        {
            return $"{nameof(NullOr<T>)}{{ Value = {Value}, IsNull = {IsNull} }}";
        }
    }
}
