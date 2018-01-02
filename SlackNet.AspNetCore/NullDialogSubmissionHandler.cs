using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    public class NullDialogSubmissionHandler : IDialogSubmissionHandler
    {
        public Task<IEnumerable<DialogError>> Handle(DialogSubmission dialog) => Task.FromResult(Enumerable.Empty<DialogError>());
    }
}