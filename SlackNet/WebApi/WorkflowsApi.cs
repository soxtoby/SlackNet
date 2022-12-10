using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using SlackNet.Interaction;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IWorkflowsApi
{
    /// <summary>
    /// Update the configuration for a workflow extension step.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/workflows.updateStep">Slack documentation</a> for more information.</remarks>
    /// <param name="workflowStepEditId">The <see cref="EditedWorkflowStep.WorkflowStepEditId"/> provided with a <see cref="ViewSubmission"/> payload.</param>
    /// <param name="inputs">
    /// A map of inputs required from a user during configuration.
    /// This is the data your app expects to receive when the workflow step starts.
    /// </param>
    /// <param name="outputs">
    /// A list of output objects used during step execution.
    /// This is the data your app agrees to provide when your workflow step was executed.
    /// </param>
    /// <param name="stepImageUrl">An optional parameter that can be used to override app image that is shown in the Workflow Builder.</param>
    /// <param name="stepName">An optional parameter that can be used to override the step name that is shown in the Workflow Builder.</param>
    /// <param name="cancellationToken"></param>
    Task UpdateStep(string workflowStepEditId, IDictionary<string, WorkflowInput> inputs, IEnumerable<WorkflowOutput> outputs, string stepImageUrl = null, string stepName = null, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Indicate that an app's step in a workflow completed execution.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/workflows.stepCompleted">Slack documentation</a> for more information.</remarks>
    /// <param name="workflowStepExecuteId">The <see cref="WorkflowStep.WorkflowStepExecuteId"/> provided with a <see cref="WorkflowStepExecute"/> event.</param>
    /// <param name="outputs">
    /// Map of outputs from your step.
    /// Keys of this object reflect the <see cref="WorkflowOutput.Name"/>s of your outputs from your <see cref="WorkflowStep"/> object.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task StepCompleted(string workflowStepExecuteId, IDictionary<string, string> outputs, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Indicate that an app's step in a workflow failed to execute.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/workflows.stepFailed">Slack documentation</a> for more information.</remarks>
    /// <param name="workflowStepExecuteId">The <see cref="WorkflowStep.WorkflowStepExecuteId"/> provided with a <see cref="WorkflowStepExecute"/> event.</param>
    /// <param name="error">An object with a message property that should contain a human readable error message.</param>
    /// <param name="cancellationToken"></param>
    Task StepFailed(string workflowStepExecuteId, WorkflowError error, CancellationToken? cancellationToken = null);
}

public class WorkflowsApi : IWorkflowsApi
{
    private readonly ISlackApiClient _client;
    public WorkflowsApi(ISlackApiClient client) => _client = client;

    public Task UpdateStep(string workflowStepEditId,
        IDictionary<string, WorkflowInput> inputs,
        IEnumerable<WorkflowOutput> outputs,
        string stepImageUrl = null,
        string stepName = null,
        CancellationToken? cancellationToken = null
    ) =>
        _client.Post("workflows.updateStep", new Args
            {
                { "workflow_step_edit_id", workflowStepEditId },
                { "inputs", inputs },
                { "outputs", outputs },
                { "step_image_url", stepImageUrl },
                { "step_name", stepName }
            }, cancellationToken);

    public Task StepCompleted(string workflowStepExecuteId, IDictionary<string, string> outputs, CancellationToken? cancellationToken = null) =>
        _client.Get("workflows.stepCompleted", new Args
            {
                { "workflow_step_execute_id", workflowStepExecuteId },
                { "outputs", outputs }
            }, cancellationToken);

    public Task StepFailed(string workflowStepExecuteId, WorkflowError error, CancellationToken? cancellationToken = null) =>
        _client.Post("workflows.stepFailed", new Args
            {
                { "workflow_step_execute_id", workflowStepExecuteId },
                { "error", error }
            }, cancellationToken);
}