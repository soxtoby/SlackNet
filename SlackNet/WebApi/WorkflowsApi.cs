using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IWorkflowsApi
    {
        /// <summary>
        /// Update the configuration for a workflow extension step.
        /// </summary>
        /// <param name="outputs">A JSON array of output objects used during step execution.
        /// This is the data your app agrees to provide when your workflow step was executed.</param>
        /// <param name="workflowStepEditId">A context identifier provided with view_submission payloads</param>
        /// <param name="inputs"> A JSON key-value map of inputs required from a user during configuration.
        /// This is the data your app expects to receive when the workflow step starts.</param>
        /// <param name="cancellationToken"></param>
        Task UpdateStep(string workflowStepEditId, IDictionary<string, WorkflowInput> inputs, IList<WorkflowOutput> outputs, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Indicate that an app's step in a workflow completed execution.
        /// </summary>
        /// <param name="workflowStepExecuteId">Context identifier that maps to the correct workflow step execution.</param>
        /// <param name="outputs">Key-value object of outputs from your step.
        /// Keys of this object reflect the configured key properties of your outputs array from your workflow_step object.</param>
        /// <param name="cancellationToken"></param>
        Task StepCompleted(string workflowStepExecuteId, Args outputs, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Indicate that an app's step in a workflow failed to execute.
        /// </summary>
        /// <param name="workflowStepExecuteId">Context identifier that maps to the correct workflow step execution.</param>
        /// <param name="error">A JSON-based object with a message property that should contain a human readable error message.</param>
        /// <param name="cancellationToken"></param>
        Task StepFailed(string workflowStepExecuteId, Error error, CancellationToken? cancellationToken = null);
    }

    public class WorkflowsApi : IWorkflowsApi
    {
        private readonly ISlackApiClient _client;
        public WorkflowsApi(ISlackApiClient client) => _client = client;

        public Task UpdateStep(string workflowStepEditId, IDictionary<string, WorkflowInput> inputs, IList<WorkflowOutput> outputs, CancellationToken? cancellationToken = null)
        => _client.Post("workflows.updateStep", new Args
        {
            { "workflow_step_edit_id", workflowStepEditId },
            { "inputs", inputs },
            { "outputs", outputs }
        }, cancellationToken);

        public Task StepCompleted(string workflowStepExecuteId, Args outputs, CancellationToken? cancellationToken = null)
            => _client.Get("workflows.stepCompleted", new Args
        {
            { "workflow_step_execute_id", workflowStepExecuteId },
            { "outputs", outputs }
        }, cancellationToken);

        public Task StepFailed(string workflowStepExecuteId, Error error, CancellationToken? cancellationToken = null)
            => _client.Post("workflows.stepFailed", new Args
            {
                { "workflow_step_execute_id", workflowStepExecuteId },
                { "error", error }
            }, cancellationToken);
    }
}