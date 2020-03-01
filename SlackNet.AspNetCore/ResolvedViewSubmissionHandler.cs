using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    abstract class ResolvedViewSubmissionHandler : IViewSubmissionHandler
    {
        protected ResolvedViewSubmissionHandler(string callbackId) => CallbackId = callbackId;
        
        public string CallbackId { get; }
        
        public abstract Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission);
        public abstract Task HandleClose(ViewClosed viewClosed);
    }
    
    class ResolvedViewSubmissionHandler<T> : ResolvedViewSubmissionHandler
        where T : IViewSubmissionHandler
    {
        private readonly IServiceProvider _serviceProvider;
        public ResolvedViewSubmissionHandler(IServiceProvider serviceProvider, string callbackId) 
            : base(callbackId) 
            => _serviceProvider = serviceProvider;

        public override async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<T>();
                return await handler.Handle(viewSubmission).ConfigureAwait(false);
            }
        }

        public override async Task HandleClose(ViewClosed viewClosed)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<T>();
                await handler.HandleClose(viewClosed).ConfigureAwait(false);
            }
        }
    }
}