using System;
using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.AspNetCore
{
    class ResolvedEventHandler : ResolvedHandler<IEventHandler>, IEventHandler
    {
        public ResolvedEventHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IEventHandler> getHandler) 
            : base(serviceProvider, getHandler) { }

        public Task Handle(EventCallback eventCallback) => ResolvedHandle(h => h.Handle(eventCallback));
    }

    class ResolvedEventHandler<TEvent> : ResolvedHandler<IEventHandler<TEvent>>, IEventHandler
        where TEvent : Event
    {
        public ResolvedEventHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IEventHandler<TEvent>> getHandler)
            : base(serviceProvider, getHandler) { }

        public Task Handle(EventCallback eventCallback) =>
            eventCallback.Event is TEvent slackEvent
                ? ResolvedHandle(h => h.Handle(slackEvent))
                : Task.CompletedTask;
    }
}