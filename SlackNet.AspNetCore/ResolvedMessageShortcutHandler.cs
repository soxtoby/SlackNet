using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class ResolvedMessageShortcutHandler : IAsyncMessageShortcutHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<IServiceProvider, IAsyncMessageShortcutHandler> _getHandler;

        public ResolvedMessageShortcutHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IAsyncMessageShortcutHandler> getHandler)
        {
            _serviceProvider = serviceProvider;
            _getHandler = getHandler;
        }

        public async Task Handle(MessageShortcut request, Responder respond)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = _getHandler(scope.ServiceProvider);
                await handler.Handle(request, respond).ConfigureAwait(false);
            }
        }
    }
}