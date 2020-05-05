using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SlackNet.AspNetCore
{
    abstract class ResolvedHandler<THandler>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<IServiceProvider, THandler> _getHandler;

        protected ResolvedHandler(IServiceProvider serviceProvider, Func<IServiceProvider, THandler> getHandler)
        {
            _serviceProvider = serviceProvider;
            _getHandler = getHandler;
        }

        protected Task ResolvedHandle(Func<THandler, Task> handle) =>
            ResolvedHandle(async handler =>
            {
                await handle(handler).ConfigureAwait(false);
                return 0;
            });

        protected async Task<TResponse> ResolvedHandle<TResponse>(Func<THandler, Task<TResponse>> handle)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = _getHandler(scope.ServiceProvider);
                return await handle(handler).ConfigureAwait(false);
            }
        }
    }
}