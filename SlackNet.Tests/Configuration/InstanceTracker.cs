using System;
using System.Collections.Generic;
using System.Linq;

namespace SlackNet.Tests.Configuration;

public class InstanceTracker
{
    private readonly Dictionary<Type, List<object>> _instances = new();

    public int AddInstance(TrackedClass instance)
    {
        var type = instance.GetType();
        if (!_instances.ContainsKey(type))
            _instances[type] = [];
        _instances[type].Add(instance);
        return _instances[type].Count;
    }

    public IEnumerable<T> GetInstances<T>() where T : TrackedClass =>
        _instances.TryGetValue(typeof(T), out var instances)
            ? instances.Cast<T>()
            : Enumerable.Empty<T>();
}

public abstract class TrackedClass
{
    private readonly int _id;

    protected TrackedClass(InstanceTracker tracker) => _id = tracker.AddInstance(this);

    public override string ToString() => $"{GetType().Name}{_id}";
}