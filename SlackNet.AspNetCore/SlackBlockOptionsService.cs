using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SlackBlockOptionsService : ISlackBlockOptions
    {
        private readonly ISlackBlockOptions _options = new SlackBlockOptions();

        public SlackBlockOptionsService(IEnumerable<ResolvedBlockOptionProvider> providers)
        {
            foreach (var provider in providers)
                _options.SetProvider(provider.ActionName, provider);
        }

        public Task<BlockOptionsResponse> Handle(BlockOptionsRequest request) => _options.Handle(request);
        public void SetProvider(string actionId, IBlockOptionProvider handler) => _options.SetProvider(actionId, handler);
    }
}