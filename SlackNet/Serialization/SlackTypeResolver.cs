using System;
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
    private Dictionary<Type, Dictionary<string, Type>> _typeLookups = new();
    private object _lock = new object();
    public SlackTypeResolver(params Assembly[] assemblies) => _assemblies = assemblies;

    public Type FindType(Type baseType, string slackType)
    {
        Dictionary<string, Type> typeLookup;
        if (!_typeLookups.TryGetValue(baseType, out typeLookup))
        {
            lock (_lock)
            {
                if (!_typeLookups.TryGetValue(baseType, out typeLookup))
                {
                    var copy = _typeLookups.ToDictionary(x => x.Key, x => x.Value);
                    typeLookup = copy[baseType] = CreateLookup(baseType);
                    _typeLookups = copy;
                }
            }
        }

        return typeLookup.TryGetValue(slackType, out var type)
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