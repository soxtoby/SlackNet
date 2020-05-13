using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class SwitchingViewSubmissionHandler : IAsyncViewSubmissionHandler
    {
        private readonly Dictionary<string, IAsyncViewSubmissionHandler> _handlers;
        public SwitchingViewSubmissionHandler(IEnumerable<KeyedItem<IAsyncViewSubmissionHandler>> handlers) => 
            _handlers = handlers.ToDictionary(h => h.Key, h => h.Item);

        public Task Handle(ViewSubmission viewSubmission, Responder<ViewSubmissionResponse> respond) =>
            _handlers.TryGetValue(viewSubmission.View.CallbackId, out var handler)
                ? handler.Handle(viewSubmission, respond)
                : Task.CompletedTask;

        public Task HandleClose(ViewClosed viewClosed, Responder respond) =>
            _handlers.TryGetValue(viewClosed.View.CallbackId, out var handler)
                ? handler.HandleClose(viewClosed, respond)
                : Task.CompletedTask;
    }
}