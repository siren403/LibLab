// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEngine;

[Serializable]
public abstract class Control
{
    public abstract Component Source { get; }
    public abstract Type ValueType { get; }
    public abstract ChangeResult OnValueChanged();
}

public enum ChangeResult
{
    NotDirty,
    Success,
    Failure
}

[Serializable]
public abstract class Control<TSource, TValue> : Control where TSource : Component
{
    [SerializeField]
    private SourceSelector<TSource> source;

    [SerializeField]
    private TValue value;

    public override Type ValueType => typeof(TValue);

    public sealed override Component Source => source.Source;

    public sealed override ChangeResult OnValueChanged()
    {
        if (!source.HasSource)
        {
            Debug.LogWarning($"Control.OnValueChanged({source}, {value}): source is null");
            return ChangeResult.Failure;
        }
        return OnValueChanged(source.Source, value);
    }


    protected abstract ChangeResult OnValueChanged(TSource source, TValue value);

    public override string ToString()
    {
        return $"{value}";
    }
}
