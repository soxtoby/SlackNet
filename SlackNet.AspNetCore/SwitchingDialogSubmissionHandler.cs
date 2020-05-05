using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SwitchingDialogSubmissionHandler : IDialogSubmissionHandler
    {
        private readonly Dictionary<string, IDialogSubmissionHandler> _handlers;
        public SwitchingDialogSubmissionHandler(IEnumerable<KeyedItem<IDialogSubmissionHandler>> handlers) => 
            _handlers = handlers.ToDictionary(h => h.Key, h => h.Item);

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