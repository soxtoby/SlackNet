using System;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SlackNet
{
    public static class Default
    {
        public static IHttp Http(SlackJsonSettings jsonSettings = null, Func<HttpClient> getHttpClient = null) => new Http(getHttpClient ?? HttpClientFactory(), jsonSettings ?? JsonSettings());

        private static Func<HttpClient> HttpClientFactory()
        {
            var httpClient = new HttpClient();
            return () => httpClient;
        }

        public static ISlackUrlBuilder UrlBuilder(SlackJsonSettings jsonSettings = null) => new SlackUrlBuilder(jsonSettings ?? JsonSettings());

        public static SlackJsonSettings JsonSettings(ISlackTypeResolver slackTypeResolver = null) => new(SerializerSettings(slackTypeResolver ?? SlackTypeResolver()));

        private static JsonSerializerSettings SerializerSettings(ISlackTypeResolver slackTypeResolver)
        {
            var namingStrategy = new SnakeCaseNamingStrategy();
            return new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "yyyy-MM-dd",
                    ContractResolver = new SlackNetContractResolver
                        {
                            NamingStrategy = namingStrategy,
                        },
                    Converters =
                        {
                            new EnumNameConverter(namingStrategy),
                            new TimeSpanConverter(),
                            new SlackTypeConverter(slackTypeResolver)
                        }
                };
        }

        public static ISlackTypeResolver SlackTypeResolver() => new SlackTypeResolver(AssembliesContainingSlackTypes);

        public static ISlackTypeResolver SlackTypeResolver(params Assembly[] assembliesContainingSlackTypes) => new SlackTypeResolver(assembliesContainingSlackTypes);

        public static Assembly[] AssembliesContainingSlackTypes => new[] { typeof(Default).GetTypeInfo().Assembly };

        public static IWebSocketFactory WebSocketFactory => new WebSocketFactory();

        public static ISlackRequestListener RequestListener => NullRequestListener.Instance;

        public static IScheduler Scheduler => System.Reactive.Concurrency.Scheduler.Default;

        public static void RegisterServices(Action<Type, Func<Func<Type, object>, object>> registerService)
        {
            registerService(typeof(IHttp), resolve => Http((SlackJsonSettings)resolve(typeof(SlackJsonSettings))));
            registerService(typeof(ISlackUrlBuilder), resolve => UrlBuilder((SlackJsonSettings)resolve(typeof(SlackJsonSettings))));
            registerService(typeof(SlackJsonSettings), resolve => JsonSettings((ISlackTypeResolver)resolve(typeof(ISlackTypeResolver))));
            registerService(typeof(ISlackTypeResolver), resolve => SlackTypeResolver(AssembliesContainingSlackTypes));
            registerService(typeof(IWebSocketFactory), resolve => WebSocketFactory);
        }
    }
}