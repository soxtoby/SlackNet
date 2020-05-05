using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedMessageShortcutHandler : IMessageShortcutHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<IServiceProvider, IMessageShortcutHandler> _getHandler;

        public ResolvedMessageShortcutHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IMessageShortcutHandler> getHandler)
        {
            _serviceProvider = serviceProvider;
            _getHandler = getHandler;
        }

        public async Task Handle(MessageShortcut request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = _getHandler(scope.ServiceProvider);
                await handler.Handle(request).ConfigureAwait(false);
            }
        }
    }
}