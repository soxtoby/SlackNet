using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    abstract class ResolvedMessageActionHandler : IMessageActionHandler
    {
        protected ResolvedMessageActionHandler(string callbackId) => CallbackId = callbackId;

        public string CallbackId { get; }

        public abstract Task<MessageActionResponse> Handle(MessageAction request);
    }

    class ResolvedMessageActionHandler<T> : ResolvedMessageActionHandler
        where T : IMessageActionHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public ResolvedMessageActionHandler(IServiceProvider serviceProvider, string callbackId)
            : base(callbackId)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task<MessageActionResponse> Handle(MessageAction request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<T>();
                return await handler.Handle(request).ConfigureAwait(false);
            }
        }
    }
}