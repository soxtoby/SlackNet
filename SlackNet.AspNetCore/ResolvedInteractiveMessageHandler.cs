using System;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedInteractiveMessageHandler : ResolvedHandler<IInteractiveMessageHandler>, IInteractiveMessageHandler
    {
        public ResolvedInteractiveMessageHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IInteractiveMessageHandler> getHandler) 
            : base(serviceProvider, getHandler) { }

        public Task<MessageResponse> Handle(InteractiveMessage message) => ResolvedHandle(h => h.Handle(message));
    }
}