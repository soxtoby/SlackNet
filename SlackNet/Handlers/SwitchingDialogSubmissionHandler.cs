using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.Handlers
{
    public class SwitchingDialogSubmissionHandler : IDialogSubmissionHandler
    {
        private readonly IHandlerIndex<IDialogSubmissionHandler> _handlers;
        public SwitchingDialogSubmissionHandler(IHandlerIndex<IDialogSubmissionHandler> handlers) => _handlers = handlers;

        public Task<IEnumerable<DialogError>> Handle(DialogSubmission dialog) =>
            _handlers.TryGetValue(dialog.CallbackId, out var handler)
                ? handler.Handle(dialog)
                : Task.FromResult(Enumerable.Empty<DialogError>());

        public Task HandleCancel(DialogCancellation cancellation) =>
            _handlers.TryGetValue(cancellation.CallbackId, out var handler)
                ? handler.HandleCancel(cancellation)
                : Task.CompletedTask;
    }
}