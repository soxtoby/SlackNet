using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using SlackNet.Rtm;

namespace SlackNet;

class SlackTypeResolver(
    IReadOnlyDictionary<Type, Type> replacedSlackTypes, 
    IEnumerable<(Type Type, string TypeIdentifier)> registeredSlackTypes)
{
    private static readonly Assembly SlackNetAssembly = typeof(SlackTypeResolver).Assembly;
    private Dictionary<Type, Dictionary<string, Type>> _typeLookups = new();
    private readonly object _lock = new();

    public SlackTypeResolver() : this(new Dictionary<Type, Type>(), []) { }

    /// <summary>
    /// Finds the type to deserialize a Slack object into, where SlackNet expects an <paramref name="expectedType"/>.
    /// </summary>
    public Type FindType(Type expectedType, JObject slackObject)
    {
        var identifiedType = FindIdentifiedType(expectedType, slackObject);

        // Replacements are found last, so that subtypes are still looked up from SlackNet's own types
        return replacedSlackTypes.TryGetValue(identifiedType, out var replacement)
            ? replacement
            : identifiedType;
    }

    /// <summary>
    /// Finds the type that <paramref name="typeIdentifier"/> already resolves to, alongside the types that <paramref name="type"/> derives from.
    /// </summary>
    public Type FindExistingType(Type type, string typeIdentifier)
    {
        var baseType = NearestSlackNetBaseType(type);
        return baseType != null && Lookup(baseType).TryGetValue(typeIdentifier, out var existingType)
            ? existingType
            : null;
    }

    private Type FindIdentifiedType(Type expectedType, JObject slackObject)
    {
        if (slackObject.Value<uint>("reply_to") > 0)
            return typeof(Reply);

        var type = FindSubtype(expectedType, slackObject.Value<string>("type"));

        var subtypeProperty = type.GetCustomAttribute<SlackSubTypePropertyAttribute>()?.PropertyName.SnakeCase()
            ?? "subtype";

        return FindSubtype(type, slackObject.Value<string>(subtypeProperty));
    }

    private Type FindSubtype(Type baseType, string typeIdentifier) =>
        typeIdentifier != null && Lookup(baseType).TryGetValue(typeIdentifier, out var type)
            ? type
            : baseType;

    private static Type NearestSlackNetBaseType(Type type) =>
        type.BaseType == null || type.BaseType.Assembly == SlackNetAssembly
            ? type.BaseType
            : NearestSlackNetBaseType(type.BaseType);

    private Dictionary<string, Type> Lookup(Type baseType)
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

        return typeLookup;
    }

    private Dictionary<string, Type> CreateLookup(Type baseType) =>
        SlackNetAssembly.ExportedTypes
            .Where(baseType.IsAssignableFrom)
            .Select(t => (Type: t, TypeIdentifier: t.GetTypeInfo().SlackType()))
            .Concat(registeredSlackTypes.Where(t => baseType.IsAssignableFrom(t.Type)))
            .GroupBy(t => t.TypeIdentifier, t => t.Type)
            .ToDictionary(g => g.Key, g => MostSpecific(g.ToList()));

    // A type replaces any type it derives from (e.g. MessageShortcut replaces MessageAction).
    // Otherwise, the type closest to the base type wins, since some message subtypes share a name with an event (e.g. channel_archive).
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
