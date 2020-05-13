using System;
using System.Threading.Tasks;

namespace SlackNet.Interaction.Experimental
{
    [Obsolete(Warning.Experimental)]
    public interface IAsyncViewSubmissionHandler
    {
        Task Handle(ViewSubmission viewSubmission, Responder<ViewSubmissionResponse> respond);
        Task HandleClose(ViewClosed viewClosed, Responder respond);
    }
}