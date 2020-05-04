
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedMessageShortcutHandler<T> : IMessageShortcutHandler
        where T : IMessageShortcutHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public ResolvedMessageShortcutHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(MessageShortcut request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<T>();
                await handler.Handle(request).ConfigureAwait(false);
            }
        }
    }
}