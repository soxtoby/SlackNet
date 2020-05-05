using System;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedBlockOptionProvider : ResolvedHandler<IBlockOptionProvider>, IBlockOptionProvider
    {
        public ResolvedBlockOptionProvider(IServiceProvider serviceProvider, Func<IServiceProvider, IBlockOptionProvider> getProvider)
            : base(serviceProvider, getProvider) { }

        public Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request) => ResolvedHandle(h => h.GetOptions(request));
    }
}