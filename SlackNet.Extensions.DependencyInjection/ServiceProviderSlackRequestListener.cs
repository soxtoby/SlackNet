using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SlackNet.Extensions.DependencyInjection;

public interface IServiceProviderSlackRequestListener : ISlackRequestListener;

class ServiceProviderSlackRequestListener : IServiceProviderSlackRequestListener
{
    private readonly IServiceProvider _serviceProvider;
    public ServiceProviderSlackRequestListener(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public void OnRequestBegin(SlackRequestContext context)
    {
        var scope = _serviceProvider.CreateScope();
        context.SetServiceProvider(scope.ServiceProvider);
        context.OnComplete(() =>
            {
                scope.Dispose();
                return Task.CompletedTask;
            });
    }
}