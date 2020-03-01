using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SlackViewsService: ISlackViews
    {
        private readonly ISlackViews _views = new SlackViews();
        
        public SlackViewsService(IEnumerable<ResolvedViewSubmissionHandler> handlers)
        {
            foreach (var handler in handlers)
                SetHandler(handler.CallbackId, handler);
        }

        public Task<ViewSubmissionResponse> HandleSubmission(ViewSubmission viewSubmission) => _views.HandleSubmission(viewSubmission);
        public Task HandleClose(ViewClosed viewClosed) => _views.HandleClose(viewClosed);
        public void SetHandler(string callbackId, IViewSubmissionHandler handler) => _views.SetHandler(callbackId, handler);
    }
}