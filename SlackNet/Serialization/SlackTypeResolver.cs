using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SlackNet;

public interface ISlackTypeResolver
{
    Type FindType(Type baseType, string slackType);
}

class SlackTypeResolver : ISlackTypeResolver
{
    private readonly Assembly[] _assemblies;
    private readonly ConcurrentDictionary<Type, Dictionary<string, Type>> _typeLookups = new();

    public SlackTypeResolver(params Assembly[] assemblies) => _assemblies = assemblies;

    public Type FindType(Type baseType, string slackType)
    {
        return _typeLookups.GetOrAdd(baseType, bt => CreateLookup(bt)).TryGetValue(slackType, out var type)
            ? type
            : baseType;

    }

    private Dictionary<string, Type> CreateLookup(Type baseType)
    {
        var lookup = new Dictionary<string, Type>();
        _assemblies
            .SelectMany(a => a.ExportedTypes)
            .Select(t => t.GetTypeInfo())
            .Where(t => baseType.GetTypeInfo().IsAssignableFrom(t))
            .Select(t => new { type = t.AsType(), slackType = t.SlackType() })
            .ToList()
            .ForEach(t => lookup[t.slackType] = t.type);
        return lookup;
    }
}