namespace SlackNet.Handlers
{
    public interface ISlackServiceFactory
    {
        IHttp GetHttp();
        SlackJsonSettings GetJsonSettings();
        ISlackTypeResolver GetTypeResolver();
        ISlackUrlBuilder GetUrlBuilder();
        IWebSocketFactory GetWebSocketFactory();
        ISlackHandlerFactory GetHandlerFactory();
        ISlackApiClient GetApiClient();
    }
}