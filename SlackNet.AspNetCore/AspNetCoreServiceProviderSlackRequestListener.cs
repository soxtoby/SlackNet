using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Extensions.DependencyInjection;

namespace SlackNet.AspNetCore
{
    class AspNetCoreServiceProviderSlackRequestListener : IServiceProviderSlackRequestListener
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public AspNetCoreServiceProviderSlackRequestListener(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public void OnRequestBegin(SlackRequestContext context)
        {
            if (context.ContainsKey("Envelope")) // Socket mode
            {
                var scope = _serviceProvider.CreateScope();
                context.SetServiceProvider(scope.ServiceProvider);
                context.OnComplete(() =>
                    {
                        scope.Dispose();
                        return Task.CompletedTask;
                    });
            }
            else
            {
                context.SetServiceProvider(_httpContextAccessor.HttpContext.RequestServices);
            }
        }
    }
}