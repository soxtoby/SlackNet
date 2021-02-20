using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public class SwitchingViewSubmissionHandler : IAsyncViewSubmissionHandler
    {
        private readonly IHandlerIndex<IAsyncViewSubmissionHandler> _handlers;
        public SwitchingViewSubmissionHandler(IHandlerIndex<IAsyncViewSubmissionHandler> handlers) => _handlers = handlers;

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