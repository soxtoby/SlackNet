using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface ISlackBlockOptions
    {
        Task<BlockOptionsResponse> Handle(BlockOptionsRequest request);
        void SetProvider(string actionId, IBlockOptionProvider handler);
    }

    public class SlackBlockOptions : ISlackBlockOptions
    {
        private readonly Dictionary<string, IBlockOptionProvider> _providers = new Dictionary<string, IBlockOptionProvider>();

        public Task<BlockOptionsResponse> Handle(BlockOptionsRequest request) =>
            _providers.TryGetValue(request.ActionId, out var provider)
                ? provider.GetOptions(request)
                : Task.FromResult(new BlockOptionsResponse());

        public void SetProvider(string actionId, IBlockOptionProvider provider) => _providers[actionId] = provider;
    }
}