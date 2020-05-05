using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class CompositeBlockActionHandler : IBlockActionHandler
    {
        private readonly List<CompositeItem<IBlockActionHandler>> _handlers;
        public CompositeBlockActionHandler(IEnumerable<CompositeItem<IBlockActionHandler>> handlers) => _handlers = handlers.ToList();

        public Task Handle(BlockActionRequest request) => Task.WhenAll(_handlers.Select(h => h.Item.Handle(request)));
    }
}