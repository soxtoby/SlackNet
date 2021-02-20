using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.Handlers
{
    public class SwitchingBlockOptionProvider : IBlockOptionProvider
    {
        private readonly IHandlerIndex<IBlockOptionProvider> _providers;
        public SwitchingBlockOptionProvider(IHandlerIndex<IBlockOptionProvider> providers) => _providers = providers;

        public Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request) =>
            _providers.TryGetValue(request.ActionId, out var provider)
                ? provider.GetOptions(request)
                : Task.FromResult(new BlockOptionsResponse());
    }
}