using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers;

class ViewSubmissionHandlerAsyncWrapper : IAsyncViewSubmissionHandler, IComposedHandler<ViewSubmission>, IComposedHandler<ViewClosed>
{
    private readonly IViewSubmissionHandler _syncHandler;
    public ViewSubmissionHandlerAsyncWrapper(IViewSubmissionHandler syncHandler) => _syncHandler = syncHandler;

    public async Task Handle(ViewSubmission viewSubmission, Responder<ViewSubmissionResponse> respond)
    {
        var result = await _syncHandler.Handle(viewSubmission).ConfigureAwait(false);
        if (result != null)
            await respond(result).ConfigureAwait(false);
    }

    public Task HandleClose(ViewClosed viewClosed, Responder respond) => _syncHandler.HandleClose(viewClosed);

    IEnumerable<object> IComposedHandler<ViewSubmission>.InnerHandlers(ViewSubmission request) => _syncHandler.InnerHandlers(request);
    IEnumerable<object> IComposedHandler<ViewClosed>.InnerHandlers(ViewClosed request) => _syncHandler.InnerHandlers(request);
}