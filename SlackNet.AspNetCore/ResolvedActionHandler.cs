using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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

        public override Task<MessageResponse> Handle(InteractiveMessage message)
        {
            var handler = _serviceProvider.GetRequiredService<T>();
            return handler.Handle(message);
        }
    }
}