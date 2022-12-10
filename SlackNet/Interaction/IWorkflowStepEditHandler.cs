using System.Threading.Tasks;

namespace SlackNet.Interaction;

public interface IWorkflowStepEditHandler
{
    Task Handle(WorkflowStepEdit workflowStepEdit);
}