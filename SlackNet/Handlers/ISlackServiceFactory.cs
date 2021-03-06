namespace SlackNet.Handlers
{
    public interface ISlackServiceFactory
    {
        IHttp GetHttp();
        SlackJsonSettings GetJsonSettings();
        ISlackTypeResolver GetTypeResolver();
        ISlackUrlBuilder GetUrlBuilder();
        IWebSocketFactory GetWebSocketFactory();
        ISlackRequestContextFactory GetRequestContextFactory();
        ISlackRequestListener GetRequestListener();
        ISlackHandlerFactory GetHandlerFactory();
        ISlackApiClient GetApiClient();
        ISlackSocketModeClient GetSocketModeClient();
    }
}