using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Events;

namespace SlackNet.AspNetCore
{
    class ResolvedEventHandler<TEvent, THandler> : IEventHandler<TEvent>
        where TEvent : Event
        where THandler : IEventHandler<TEvent>
    {
        private readonly IServiceProvider _serviceProvider;

        public ResolvedEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(TEvent slackEvent)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<THandler>();
                await handler.Handle(slackEvent).ConfigureAwait(false);
            }
        }
    }
}