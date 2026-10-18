#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IFunctionWorkflowStepsApi
{
    /// <summary>
    /// List the steps of a specific function of a workflow's versions.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/functions.workflows.steps.list">Slack documentation</a> for more information.</remarks>
    /// <param name="functionId">The ID of the function to query.</param>
    /// <param name="workflowId">The workflow ID, starts with Wf*.</param>
    /// <param name="workflow">The workflow encoded ID or workflow reference.</param>
    /// <param name="workflowAppId">The app tied to the workflow reference.</param>
    /// <param name="cancellationToken"></param>
    Task<WorkflowStepsVersionsResponse> List(
        string functionId,
        string? workflowId = null,
        string? workflow = null,
        string? workflowAppId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download form responses of a workflow.
    /// Exports are generated asynchronously and delivered via a direct message from Slackbot.
    /// </summary>
    /// <param name="stepId">The ID of the OpenForm step to export.</param>
    /// <param name="workflowId">The workflow ID, starts with Wf*.</param>
    /// <param name="workflow">The workflow encoded ID or workflow reference.</param>
    /// <param name="workflowAppId">The app tied to the workflow reference.</param>
    /// <param name="cancellationToken"></param>
    Task ExportResponses(
        string stepId,
        string? workflowId = null,
        string? workflow = null,
        string? workflowAppId = null,
        CancellationToken cancellationToken = default);
}

public class FunctionWorkflowStepsApi(ISlackApiClient client) : IFunctionWorkflowStepsApi
{
    public Task<WorkflowStepsVersionsResponse> List(
        string functionId,
        string? workflowId = null,
        string? workflow = null,
        string? workflowAppId = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<WorkflowStepsVersionsResponse>("functions.workflows.steps.list", new Args
            {
                { "function_id", functionId },
                { "workflow_id", workflowId },
                { "workflow", workflow },
                { "workflow_app_id", workflowAppId }
            }, cancellationToken);

    public Task ExportResponses(
        string stepId,
        string? workflowId = null,
        string? workflow = null,
        string? workflowAppId = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("functions.workflows.steps.responses.export", new Args
            {
                { "step_id", stepId },
                { "workflow_id", workflowId },
                { "workflow", workflow },
                { "workflow_app_id", workflowAppId }
            }, cancellationToken);
}