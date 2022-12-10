using System;
using System.Threading.Tasks;

namespace SlackNet.Interaction.Experimental;

[Obsolete(Warning.Experimental)]
public interface IAsyncWorkflowStepEditHandler
{
    Task Handle(WorkflowStepEdit workflowStepEdit, Responder respond);
}