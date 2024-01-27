using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Extensions.DependencyInjection;

namespace SlackNet.AspNetCore;

class AspNetCoreServiceProviderSlackRequestListener(IRequestServiceProviderAccessor serviceProviderAccessor, IServiceProvider serviceProvider)
    : IServiceProviderSlackRequestListener
{
    public void OnRequestBegin(SlackRequestContext context)
    {
        if (context.ContainsKey("Envelope")) // Socket mode
        {
            var scope = serviceProvider.CreateScope();
            context.SetServiceProvider(scope.ServiceProvider);
            context.OnComplete(() =>
                {
                    scope.Dispose();
                    return Task.CompletedTask;
                });
        }
        else
        {
            context.SetServiceProvider(serviceProviderAccessor.ServiceProvider);
        }
    }
}