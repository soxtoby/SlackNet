using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    class WorkflowStepEditHandlerAsyncWrapper : IAsyncWorkflowStepEditHandler
    {
        private readonly IWorkflowStepEditHandler _syncEditHandler;

        public WorkflowStepEditHandlerAsyncWrapper(IWorkflowStepEditHandler syncEditHandler) => _syncEditHandler = syncEditHandler;

        public Task Handle(WorkflowStepEdit workflowStep, Responder respond) => _syncEditHandler.Handle(workflowStep);
    }
}