using System;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class ResolvedViewSubmissionHandler : ResolvedHandler<IAsyncViewSubmissionHandler>, IAsyncViewSubmissionHandler
    {
        public ResolvedViewSubmissionHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IAsyncViewSubmissionHandler> getHandler)
            : base(serviceProvider, getHandler) { }

        public Task Handle(ViewSubmission viewSubmission, Responder<ViewSubmissionResponse> respond) => ResolvedHandle(h => h.Handle(viewSubmission, respond));

        public Task HandleClose(ViewClosed viewClosed, Responder respond) => ResolvedHandle(h => h.HandleClose(viewClosed, respond));
    }
}