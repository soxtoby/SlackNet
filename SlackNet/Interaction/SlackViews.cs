using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface ISlackViews
    {
        Task<ViewSubmissionResponse> HandleSubmission(ViewSubmission viewSubmission);
        Task HandleClose(ViewClosed viewClosed);
        void SetHandler(string callbackId, IViewSubmissionHandler handler);
    }

    public class SlackViews : ISlackViews
    {
        private readonly Dictionary<string, IViewSubmissionHandler> _handlers = new Dictionary<string, IViewSubmissionHandler>();

        public Task<ViewSubmissionResponse> HandleSubmission(ViewSubmission viewSubmission) =>
            _handlers.TryGetValue(viewSubmission.View.CallbackId, out var handler)
                ? handler.Handle(viewSubmission)
                : Task.FromResult(ViewSubmissionResponse.Null);

        public Task HandleClose(ViewClosed viewClosed) =>
            _handlers.TryGetValue(viewClosed.View.CallbackId, out var handler)
                ? handler.HandleClose(viewClosed)
                : Task.CompletedTask;

        public void SetHandler(string callbackId, IViewSubmissionHandler handler) => _handlers[callbackId] = handler;
    }
}