using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface IViewSubmissionHandler
    {
        Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission);
        Task HandleClose(ViewClosed viewClosed);
    }
}