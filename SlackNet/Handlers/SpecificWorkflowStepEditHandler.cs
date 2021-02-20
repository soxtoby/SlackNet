using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    class SpecificWorkflowStepEditHandler : IAsyncWorkflowStepEditHandler
    {
        private readonly string _callbackId;
        private readonly IAsyncWorkflowStepEditHandler _handler;

        public SpecificWorkflowStepEditHandler(string callbackId, IAsyncWorkflowStepEditHandler handler)
        {
            _callbackId = callbackId;
            _handler = handler;
        }

        public async Task Handle(WorkflowStepEdit request, Responder respond)
        {
            if (request.CallbackId == _callbackId)
                await _handler.Handle(request, respond).ConfigureAwait(false);
        }
    }
}