using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    abstract class ResolvedInteractiveMessageHandler : IInteractiveMessageHandler
    {
        protected ResolvedInteractiveMessageHandler(string actionName) => ActionName = actionName;

        public string ActionName { get; }

        public abstract Task<MessageResponse> Handle(InteractiveMessage message);
    }

    class ResolvedInteractiveMessageHandler<T> : ResolvedInteractiveMessageHandler
        where T : IInteractiveMessageHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public ResolvedInteractiveMessageHandler(IServiceProvider serviceProvider, string actionName)
            : base(actionName)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task<MessageResponse> Handle(InteractiveMessage message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<T>();
                return await handler.Handle(message).ConfigureAwait(false);
            }
        }
    }
}