using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    abstract class ResolvedActionHandler : IActionHandler
    {
        protected ResolvedActionHandler(string actionName) => ActionName = actionName;

        public string ActionName { get; }

        public abstract Task<MessageResponse> Handle(InteractiveMessage message);
    }

    class ResolvedActionHandler<T> : ResolvedActionHandler
        where T : IActionHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public ResolvedActionHandler(IServiceProvider serviceProvider, string actionName)
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