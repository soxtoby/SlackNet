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
    private readonly object _lock = new();
    public SlackTypeResolver(params Assembly[] assemblies) => _assemblies = assemblies;

    public Type FindType(Type baseType, string slackType)
    {
        if (!_typeLookups.TryGetValue(baseType, out var typeLookup))
        {
            lock (_lock)
            {
                if (!_typeLookups.TryGetValue(baseType, out typeLookup))
                {
                    var copy = _typeLookups.ToDictionary(kv => kv.Key, kv => kv.Value);
                    typeLookup = copy[baseType] = CreateLookup(baseType);
                    _typeLookups = copy;
                }
            }
        }

        return typeLookup.TryGetValue(slackType, out var type)
            ? type
            : baseType;

    }

    private Dictionary<string, Type> CreateLookup(Type baseType) =>
        _assemblies
            .SelectMany(a => a.ExportedTypes)
            .Where(baseType.IsAssignableFrom)
            .GroupBy(t => t.GetTypeInfo().SlackType())
            .ToDictionary(g => g.Key, g => MostSpecific(g.ToList()));

    // A type replaces any type it derives from (e.g. MessageShortcut replaces MessageAction).
    // Otherwise the type closest to the base type wins, since some message subtypes share a name with an event (e.g. channel_archive).
    private static Type MostSpecific(IList<Type> types) =>
        types
            .Where(t => !types.Any(other => other != t && t.IsAssignableFrom(other)))
            .OrderBy(InheritanceDepth)
            .First();

    private static int InheritanceDepth(Type type) =>
        type.BaseType == null
            ? 0
            : InheritanceDepth(type.BaseType) + 1;
}