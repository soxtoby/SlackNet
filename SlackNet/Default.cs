using System.Net.Http;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SlackNet
{
    public class Default
    {
        public static IHttp Http(JsonSerializerSettings serializerSettings) => new Http(new HttpClient(), serializerSettings);

        public static ISlackUrlBuilder UrlBuilder(JsonSerializerSettings serializerSettings) => new SlackUrlBuilder(serializerSettings);

        public static JsonSerializerSettings SerializerSettings(ISlackTypeResolver slackTypeResolver)
        {
            var namingStrategy = new SnakeCaseNamingStrategy();
            return new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = namingStrategy
                        },
                    Converters =
                        {
                            new EnumNameConverter(namingStrategy),
                            new SlackTypeConverter(slackTypeResolver)
                        }
                };
        }

        public static ISlackTypeResolver SlackTypeResolver(params Assembly[] assembliesContainingSlackTypes) => new SlackTypeResolver(assembliesContainingSlackTypes);

        public static Assembly[] AssembliesContainingSlackTypes => new[] { typeof(Default).GetTypeInfo().Assembly };
    }
}