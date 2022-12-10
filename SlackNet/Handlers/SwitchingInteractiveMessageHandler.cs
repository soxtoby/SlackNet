using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.Handlers;

public class SwitchingInteractiveMessageHandler : IInteractiveMessageHandler, IComposedHandler<InteractiveMessage>
{
    private readonly IHandlerIndex<IInteractiveMessageHandler> _handlers;
    public SwitchingInteractiveMessageHandler(IHandlerIndex<IInteractiveMessageHandler> handlers) => _handlers = handlers;

    public Task<MessageResponse> Handle(InteractiveMessage message) =>
        _handlers.TryGetHandler(message.Action.Name, out var handler)
            ? handler.Handle(message)
            : Task.FromResult<MessageResponse>(null);

    IEnumerable<object> IComposedHandler<InteractiveMessage>.InnerHandlers(InteractiveMessage request) =>
        _handlers.TryGetHandler(request.Action.Name, out var handler)
            ? handler.InnerHandlers(request)
            : Enumerable.Empty<object>();
}