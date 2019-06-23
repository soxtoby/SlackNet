
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedMessageActionHandler<T> : IMessageActionHandler
        where T : IMessageActionHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public ResolvedMessageActionHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(MessageAction request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<T>();
                await handler.Handle(request).ConfigureAwait(false);
            }
        }
    }
}