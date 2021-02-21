using System;

namespace SlackNet.Handlers
{
    public abstract class SlackServiceConfigurationBase<TConfig> where TConfig : SlackServiceConfigurationBase<TConfig>
    {
        public string ApiToken { get; private set; }
        public string AppLevelToken { get; private set; }

        /// <summary>
        /// Used for making Slack API requests. Can be either a bot or user token.
        /// </summary>
        public virtual TConfig UseApiToken(string token) => Chain(() => ApiToken = token);

        /// <summary>
        /// Used for connection socket mode client.
        /// </summary>
        public virtual TConfig UseAppLevelToken(string token) => Chain(() => AppLevelToken = token);

        public abstract TConfig UseHttp(Func<IHttp> httpProvider);
        public abstract TConfig UseJsonSettings(Func<SlackJsonSettings> jsonSettingsProvider);
        public abstract TConfig UseTypeResolver(Func<ISlackTypeResolver> slackTypeResolverProvider);
        public abstract TConfig UseUrlBuilder(Func<ISlackUrlBuilder> urlBuilderProvider);
        public abstract TConfig UseWebSocketFactory(Func<IWebSocketFactory> webSocketFactoryProvider);
        public abstract TConfig UseRequestListener(Func<ISlackRequestListener> requestListenerProvider);
        public abstract TConfig UseHandlerFactory(Func<ISlackHandlerFactory> handlerFactoryProvider);
        public abstract TConfig UseApiClient(Func<ISlackApiClient> apiClientProvider);
        public abstract TConfig UseSocketModeClient(Func<ISlackSocketModeClient> socketModeClientProvider);

        protected TConfig Chain(Action action)
        {
            action();
            return (TConfig)this;
        }
    }
}