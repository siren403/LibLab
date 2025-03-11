using System;
using UnityEngine;

public enum SourceLocation
{
    Origin,
    Children,
    Manual
}

[Serializable]
public abstract class SourceSelector
{
    [SerializeField]
    private SourceLocation location = SourceLocation.Origin;

    public abstract Type SourceType { get; }

    public SourceLocation Location => location;
}

[Serializable]
public class SourceSelector<T> : SourceSelector where T : Component
{
    public override Type SourceType => typeof(T);

    [SerializeField]
    private T source;

    public T Source => source;
}
