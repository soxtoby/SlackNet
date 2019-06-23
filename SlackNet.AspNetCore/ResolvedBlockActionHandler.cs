using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedBlockActionHandler<TAction, THandler> : IBlockActionHandler<TAction>
        where TAction : BlockAction 
        where THandler : IBlockActionHandler<TAction>
    {
        private readonly IServiceProvider _serviceProvider;
        public ResolvedBlockActionHandler(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task Handle(TAction action, BlockActionRequest request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<THandler>();
                await handler.Handle(action, request).ConfigureAwait(false);
            }
        }
    }
}