using System;
using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedBlockActionHandler : ResolvedHandler<IBlockActionHandler>, IBlockActionHandler
    {
        public ResolvedBlockActionHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IBlockActionHandler> getHandler) 
            : base(serviceProvider, getHandler) { }

        public Task Handle(BlockActionRequest request) => ResolvedHandle(h => h.Handle(request));
    }

    class ResolvedBlockActionHandler<TAction> : ResolvedHandler<IBlockActionHandler<TAction>>, IBlockActionHandler
        where TAction : BlockAction
    {
        public ResolvedBlockActionHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IBlockActionHandler<TAction>> getHandler) 
            : base(serviceProvider, getHandler) { }

        public Task Handle(BlockActionRequest request) =>
            request.Action is TAction action
                ? ResolvedHandle(h => h.Handle(action, request))
                : Task.CompletedTask;
    }
}