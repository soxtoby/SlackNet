using System;
using System.Collections.Generic;
using SlackNet.Handlers;

namespace SlackNet;

/// <summary>
/// A basic factory for SlackNet services, with some configuration.
/// If you're using a dependency injection library, you're probably better off integrating with that instead of using this.
/// </summary>
public class SlackServiceBuilder : SlackServiceConfigurationBase<SlackServiceBuilder>, ISlackServiceProvider
{
    private readonly Lazy<ISlackServiceProvider> _factory;

    public SlackServiceBuilder() => _factory = new Lazy<ISlackServiceProvider>(() => CreateServiceFactory(this));

    public IHttp GetHttp() => _factory.Value.GetHttp();
    public SlackJsonSettings GetJsonSettings() => _factory.Value.GetJsonSettings();
    public ISlackTypeResolver GetTypeResolver() => _factory.Value.GetTypeResolver();
    public ISlackUrlBuilder GetUrlBuilder() => _factory.Value.GetUrlBuilder();
    public ILogger GetLogger() => _factory.Value.GetLogger();
    public IWebSocketFactory GetWebSocketFactory() => _factory.Value.GetWebSocketFactory();
    public IEnumerable<ISlackRequestListener> GetRequestListeners() => _factory.Value.GetRequestListeners();
    public ISlackHandlerFactory GetHandlerFactory() => _factory.Value.GetHandlerFactory();
    public ISlackApiClient GetApiClient() => _factory.Value.GetApiClient();
    public ISlackSocketModeClient GetSocketModeClient() => _factory.Value.GetSocketModeClient();
}