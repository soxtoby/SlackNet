using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.Handlers;

public class SwitchingDialogSubmissionHandler : IDialogSubmissionHandler, IComposedHandler<DialogSubmission>, IComposedHandler<DialogCancellation>
{
    private readonly IHandlerIndex<IDialogSubmissionHandler> _handlers;
    public SwitchingDialogSubmissionHandler(IHandlerIndex<IDialogSubmissionHandler> handlers) => _handlers = handlers;

    public Task<IEnumerable<DialogError>> Handle(DialogSubmission dialog) =>
        _handlers.TryGetHandler(dialog.CallbackId, out var handler)
            ? handler.Handle(dialog)
            : Task.FromResult(Enumerable.Empty<DialogError>());

    public Task HandleCancel(DialogCancellation cancellation) =>
        _handlers.TryGetHandler(cancellation.CallbackId, out var handler)
            ? handler.HandleCancel(cancellation)
            : Task.CompletedTask;

    IEnumerable<object> IComposedHandler<DialogSubmission>.InnerHandlers(DialogSubmission request) =>
        _handlers.TryGetHandler(request.CallbackId, out var handler)
            ? handler.InnerHandlers(request)
            : Enumerable.Empty<object>();

    IEnumerable<object> IComposedHandler<DialogCancellation>.InnerHandlers(DialogCancellation request) =>
        _handlers.TryGetHandler(request.CallbackId, out var handler)
            ? handler.InnerHandlers(request)
            : Enumerable.Empty<object>();
}