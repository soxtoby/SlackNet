using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class CompositeBlockActionHandler : IAsyncBlockActionHandler
    {
        private readonly List<CompositeItem<IAsyncBlockActionHandler>> _handlers;
        public CompositeBlockActionHandler(IEnumerable<CompositeItem<IAsyncBlockActionHandler>> handlers) => _handlers = handlers.ToList();

        public Task Handle(BlockActionRequest request, Responder respond) => Task.WhenAll(_handlers.Select(h => h.Item.Handle(request, respond)));
    }
}