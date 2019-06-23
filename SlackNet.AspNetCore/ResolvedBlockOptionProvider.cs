using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    abstract class ResolvedBlockOptionProvider : IBlockOptionProvider
    {
        protected ResolvedBlockOptionProvider(string actionId) => ActionName = actionId;

        public string ActionName { get; }

        public abstract Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request);
    }

    class ResolvedBlockOptionProvider<T> : ResolvedBlockOptionProvider
        where T : IBlockOptionProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ResolvedBlockOptionProvider(IServiceProvider serviceProvider, string actionId)
            : base(actionId)
        {
            _serviceProvider = serviceProvider;
        }

        public override Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request)
        {
            var handler = _serviceProvider.GetRequiredService<T>();
            return handler.GetOptions(request);
        }
    }
}