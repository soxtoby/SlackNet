using System;
using System.Collections.Generic;
using System.Linq;

namespace SlackNet.Tests.Configuration
{
    public class InstanceTracker
    {
        private readonly Dictionary<Type, List<object>> _instances = new();

        public virtual void AddInstance(TrackedClass instance)
        {
            var type = instance.GetType();
            if (!_instances.ContainsKey(type))
                _instances[type] = new List<object>();
            _instances[type].Add(instance);
        }

        public IEnumerable<T> GetInstances<T>() where T : TrackedClass =>
            _instances.TryGetValue(typeof(T), out var instances)
                ? instances.Cast<T>()
                : Enumerable.Empty<T>();
    }

    public abstract class TrackedClass
    {
        protected TrackedClass(InstanceTracker tracker) => tracker.AddInstance(this);
    }
}