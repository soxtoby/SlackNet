using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class CompositeWorkflowStepEditHandler : IAsyncWorkflowStepEditHandler
    {
        private readonly List<CompositeItem<IAsyncWorkflowStepEditHandler>> _handlers;
        public CompositeWorkflowStepEditHandler(IEnumerable<CompositeItem<IAsyncWorkflowStepEditHandler>> handlers) => _handlers = handlers.ToList();

        public Task Handle(WorkflowStepEdit workflowStepEdit, Responder respond) => Task.WhenAll(_handlers.Select(h => h.Item.Handle(workflowStepEdit, respond)));
    }
}