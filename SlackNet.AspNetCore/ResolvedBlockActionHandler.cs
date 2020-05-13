using System;
using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class ResolvedBlockActionHandler : ResolvedHandler<IAsyncBlockActionHandler>, IAsyncBlockActionHandler
    {
        public ResolvedBlockActionHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IAsyncBlockActionHandler> getHandler) 
            : base(serviceProvider, getHandler) { }

        public Task Handle(BlockActionRequest request, Responder respond) => ResolvedHandle(h => h.Handle(request, respond));
    }

    class ResolvedBlockActionHandler<TAction> : ResolvedHandler<IAsyncBlockActionHandler<TAction>>, IAsyncBlockActionHandler
        where TAction : BlockAction
    {
        public ResolvedBlockActionHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IAsyncBlockActionHandler<TAction>> getHandler) 
            : base(serviceProvider, getHandler) { }

        public Task Handle(BlockActionRequest request, Responder respond) =>
            request.Action is TAction action
                ? ResolvedHandle(h => h.Handle(action, request, respond))
                : Task.CompletedTask;
    }
}