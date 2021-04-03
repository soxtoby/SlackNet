using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.Handlers
{
    public class SwitchingBlockOptionProvider : IBlockOptionProvider, IComposedHandler<BlockOptionsRequest>
    {
        private readonly IHandlerIndex<IBlockOptionProvider> _providers;
        public SwitchingBlockOptionProvider(IHandlerIndex<IBlockOptionProvider> providers) => _providers = providers;

        public Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request) =>
            _providers.TryGetHandler(request.ActionId, out var provider)
                ? provider.GetOptions(request)
                : Task.FromResult(new BlockOptionsResponse());

        IEnumerable<object> IComposedHandler<BlockOptionsRequest>.InnerHandlers(BlockOptionsRequest request) =>
            _providers.TryGetHandler(request.ActionId, out var provider)
                ? provider.InnerHandlers(request)
                : Enumerable.Empty<object>();
    }
}