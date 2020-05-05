using System;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedViewSubmissionHandler : ResolvedHandler<IViewSubmissionHandler>, IViewSubmissionHandler
    {
        public ResolvedViewSubmissionHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IViewSubmissionHandler> getHandler)
            : base(serviceProvider, getHandler) { }


        public Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission) => ResolvedHandle(h => h.Handle(viewSubmission));

        public Task HandleClose(ViewClosed viewClosed) => ResolvedHandle(h => h.HandleClose(viewClosed));
    }
}