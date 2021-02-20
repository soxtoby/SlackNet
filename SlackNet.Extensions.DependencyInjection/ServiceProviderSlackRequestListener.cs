using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Handlers;

namespace SlackNet.Extensions.DependencyInjection
{
    class ServiceProviderSlackRequestListener : ISlackRequestListener
    {
        private readonly IServiceProvider _serviceProvider;
        public ServiceProviderSlackRequestListener(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public Task OnRequestBegin(SlackRequestContext context)
        {
            context.SetServiceScope(_serviceProvider.CreateScope());
            return Task.CompletedTask;
        }

        public Task OnRequestEnd(SlackRequestContext context)
        {
            context.ServiceScope().Dispose();
            return Task.CompletedTask;
        }
    }
}