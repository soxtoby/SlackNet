using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface IDialogSubmissionHandler
    {
        Task<IEnumerable<DialogError>> Handle(DialogSubmission dialog);
    }
}