using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SlackNet
{
    public interface ISlackTypeResolver
    {
        Type FindType(Type baseType, string slackType);
    }

    class SlackTypeResolver : ISlackTypeResolver
    {
        private readonly Assembly[] _assemblies;
        private readonly Dictionary<Type, Dictionary<string, Type>> _typeLookups = new Dictionary<Type, Dictionary<string, Type>>();

        public SlackTypeResolver(params Assembly[] assemblies) => _assemblies = assemblies;

        public Type FindType(Type baseType, string slackType)
        {
            lock (_typeLookups)
            {
                if (!_typeLookups.TryGetValue(baseType, out var lookup))
                    lookup = _typeLookups[baseType] = CreateLookup(baseType);

                return lookup.TryGetValue(slackType, out var type)
                    ? type
                    : baseType;
            }
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
}
