using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SwitchingBlockOptionProvider : IBlockOptionProvider
    {
        private readonly Dictionary<string, IBlockOptionProvider> _providers;
        public SwitchingBlockOptionProvider(IEnumerable<KeyedItem<IBlockOptionProvider>> providers) => 
            _providers = providers.ToDictionary(p => p.Key, p => p.Item);

        public Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request) =>
            _providers.TryGetValue(request.ActionId, out var provider)
                ? provider.GetOptions(request)
                : Task.FromResult(new BlockOptionsResponse());
    }
}