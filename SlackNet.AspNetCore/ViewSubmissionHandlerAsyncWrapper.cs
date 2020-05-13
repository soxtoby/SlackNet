using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class ViewSubmissionHandlerAsyncWrapper : IAsyncViewSubmissionHandler
    {
        private readonly IViewSubmissionHandler _syncHandler;
        public ViewSubmissionHandlerAsyncWrapper(IViewSubmissionHandler syncHandler) => _syncHandler = syncHandler;

        public async Task Handle(ViewSubmission viewSubmission, Responder<ViewSubmissionResponse> respond)
        {
            var result = await _syncHandler.Handle(viewSubmission).ConfigureAwait(false);
            if (result != null)
                await respond(result).ConfigureAwait(false);
        }

        public async Task HandleClose(ViewClosed viewClosed, Responder respond)
        {
            await _syncHandler.HandleClose(viewClosed).ConfigureAwait(false);
            await respond().ConfigureAwait(false);
        }
    }
}