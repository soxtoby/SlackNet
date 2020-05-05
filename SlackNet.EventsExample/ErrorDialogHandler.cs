using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.EventsExample
{
    public class ErrorDialogHandler : IDialogSubmissionHandler
    {
        public async Task<IEnumerable<DialogError>> Handle(DialogSubmission dialog)
        {
            return dialog.Submission.Keys
                .Select(name => new DialogError { Name = name, Error = $"Not {name}-y enough!" });
        }

        public Task HandleCancel(DialogCancellation cancellation)
        {
            return Task.CompletedTask;
        }
    }
}