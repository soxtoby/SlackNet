using System;
using System.Net.Http;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SlackNet.Interaction;

namespace SlackNet
{
    public class Default
    {
        public static IHttp Http(SlackJsonSettings jsonSettings) => new Http(new HttpClient(), jsonSettings);

        public static ISlackUrlBuilder UrlBuilder(SlackJsonSettings jsonSettings) => new SlackUrlBuilder(jsonSettings);

        public static SlackJsonSettings JsonSettings(ISlackTypeResolver slackTypeResolver) => new SlackJsonSettings(SerializerSettings(slackTypeResolver));

        private static JsonSerializerSettings SerializerSettings(ISlackTypeResolver slackTypeResolver)
        {
            var namingStrategy = new SnakeCaseNamingStrategy();
            return new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = namingStrategy,
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

        public static IWebSocketFactory WebSocketFactory => new WebSocketFactory();

        public static ISlackEvents SlackEvents { get; } = new SlackEvents();

        public static ISlackActions SlackActions { get; } = new SlackActions();

        public static ISlackOptions SlackOptions { get; } = new SlackOptions();

        public static void RegisterServices(Action<Type, Func<Func<Type, object>, object>> registerService)
        {
            registerService(typeof(IHttp), resolve => Http((SlackJsonSettings)resolve(typeof(SlackJsonSettings))));
            registerService(typeof(ISlackUrlBuilder), resolve => UrlBuilder((SlackJsonSettings)resolve(typeof(SlackJsonSettings))));
            registerService(typeof(SlackJsonSettings), resolve => JsonSettings((ISlackTypeResolver)resolve(typeof(ISlackTypeResolver))));
            registerService(typeof(ISlackTypeResolver), resolve => SlackTypeResolver(AssembliesContainingSlackTypes));
            registerService(typeof(IWebSocketFactory), resolve => WebSocketFactory);
            registerService(typeof(ISlackEvents), resolve => SlackEvents);
            registerService(typeof(ISlackActions), resolve => SlackActions);
            registerService(typeof(ISlackOptions), resolve => SlackOptions);
        }
    }
}