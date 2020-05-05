using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SwitchingViewSubmissionHandler : IViewSubmissionHandler
    {
        private readonly Dictionary<string, IViewSubmissionHandler> _handlers;
        public SwitchingViewSubmissionHandler(IEnumerable<KeyedItem<IViewSubmissionHandler>> handlers) => 
            _handlers = handlers.ToDictionary(h => h.Key, h => h.Item);

        public Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission) =>
            _handlers.TryGetValue(viewSubmission.View.CallbackId, out var handler)
                ? handler.Handle(viewSubmission)
                : Task.FromResult(ViewSubmissionResponse.Null);

        public Task HandleClose(ViewClosed viewClosed) =>
            _handlers.TryGetValue(viewClosed.View.CallbackId, out var handler)
                ? handler.HandleClose(viewClosed)
                : Task.CompletedTask;
    }
}