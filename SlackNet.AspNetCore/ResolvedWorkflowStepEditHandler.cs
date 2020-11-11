using System;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class ResolvedWorkflowStepEditHandler : ResolvedHandler<IAsyncWorkflowStepEditHandler>, IAsyncWorkflowStepEditHandler
    {
        public ResolvedWorkflowStepEditHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IAsyncWorkflowStepEditHandler> getHandler)
            : base(serviceProvider, getHandler) { }

        public Task Handle(WorkflowStepEdit workflowStep, Responder respond) => ResolvedHandle(h => h.Handle(workflowStep, respond));
    }
}