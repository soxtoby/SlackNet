using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SlackNet
{
    class SlackNetContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var shouldSerializeAttributes = property.AttributeProvider.GetAttributes(typeof(ShouldSerializeAttribute), false)
                .Cast<ShouldSerializeAttribute>()
                .ToList();

            if (shouldSerializeAttributes.Any())
                property.ShouldSerialize = obj => shouldSerializeAttributes.All(a => a.ShouldSerialize(property.ValueProvider.GetValue(obj)));

            return property;
        }
    }
}