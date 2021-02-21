namespace SlackNet.Handlers
{
    public interface ISlackServiceFactory
    {
        IHttp GetHttp();
        SlackJsonSettings GetJsonSettings();
        ISlackTypeResolver GetTypeResolver();
        ISlackUrlBuilder GetUrlBuilder();
        IWebSocketFactory GetWebSocketFactory();
        ISlackRequestListener GetRequestListener();
        ISlackHandlerFactory GetHandlerFactory();
        ISlackApiClient GetApiClient();
        ISlackSocketModeClient GetSocketModeClient();
    }
}