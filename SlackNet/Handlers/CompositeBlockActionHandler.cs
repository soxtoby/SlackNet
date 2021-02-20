using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public class CompositeBlockActionHandler : IAsyncBlockActionHandler
    {
        private readonly IEnumerable<IAsyncBlockActionHandler> _handlers;
        public CompositeBlockActionHandler(IEnumerable<IAsyncBlockActionHandler> handlers) => _handlers = handlers;

        public Task Handle(BlockActionRequest request, Responder respond) => Task.WhenAll(_handlers.Select(h => h.Handle(request, respond)));
    }
}