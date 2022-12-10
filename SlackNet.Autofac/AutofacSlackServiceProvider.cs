using System;
using System.Collections.Generic;
using Autofac;
using SlackNet.Handlers;

namespace SlackNet.Autofac;

class AutofacSlackServiceProvider : ISlackServiceProvider
{
    private readonly ISlackServiceProvider _baseProvider;
    private readonly ILifetimeScope _scope;

    public AutofacSlackServiceProvider(Func<ISlackServiceProvider, ISlackServiceProvider> createBaseProvider, ILifetimeScope scope)
    {
        _baseProvider = createBaseProvider(this);
        _scope = scope;
    }

    public T Resolve<T>() => _scope.Resolve<T>();

    public T Resolve<T>(Func<IComponentContext, T> getService)
    {
        var service = getService(_scope);
        if (service is IDisposable disposable)
            _scope.Disposer.AddInstanceForDisposal(disposable);
        else if (service is IAsyncDisposable asyncDisposable)
            _scope.Disposer.AddInstanceForAsyncDisposal(asyncDisposable);
        return service;
    }

    public IHttp GetHttp() => _baseProvider.GetHttp();
    public SlackJsonSettings GetJsonSettings() => _baseProvider.GetJsonSettings();
    public ISlackTypeResolver GetTypeResolver() => _baseProvider.GetTypeResolver();
    public ISlackUrlBuilder GetUrlBuilder() => _baseProvider.GetUrlBuilder();
    public ILogger GetLogger() => _baseProvider.GetLogger();
    public IWebSocketFactory GetWebSocketFactory() => _baseProvider.GetWebSocketFactory();
    public IEnumerable<ISlackRequestListener> GetRequestListeners() => _baseProvider.GetRequestListeners();
    public ISlackHandlerFactory GetHandlerFactory() => _baseProvider.GetHandlerFactory();
    public ISlackApiClient GetApiClient() => _baseProvider.GetApiClient();
    public ISlackSocketModeClient GetSocketModeClient() => _baseProvider.GetSocketModeClient();
}