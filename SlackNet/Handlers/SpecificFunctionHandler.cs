using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Events;
using SlackNet.Handlers.Experimental;

namespace SlackNet.Handlers;

class SpecificFunctionHandler(string callbackId, Experimental.IEventHandler<Event> handler) : Experimental.IEventHandler<Event>, IComposedHandler<EventCallback>
{
    public Task Handle(IEventContext<Event> eventContext) =>
        Matches(eventContext.Callback)
            ? handler.Handle(eventContext)
            : Task.CompletedTask;

    IEnumerable<object> IComposedHandler<EventCallback>.InnerHandlers(EventCallback eventCallback) =>
        Matches(eventCallback)
            ? handler.InnerHandlers(eventCallback)
            : [];

    private bool Matches(EventCallback eventCallback) =>
        eventCallback.Event is FunctionExecuted { Function: { } function } && function.CallbackId == callbackId;
}
