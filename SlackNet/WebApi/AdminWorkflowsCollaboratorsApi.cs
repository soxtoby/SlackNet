#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages the collaborators of workflows in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminWorkflowsCollaboratorsApi
{
    /// <summary>
    /// Add collaborators to workflows within the team or enterprise.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.workflows.collaborators.add/">Slack documentation</a> for more information.</remarks>
    /// <param name="workflowIds">Array of workflow IDs to edit.</param>
    /// <param name="collaboratorIds">Array of collaborators (encoded user IDs) to add.</param>
    /// <param name="cancellationToken"></param>
    Task Add(IEnumerable<string> workflowIds, IEnumerable<string> collaboratorIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove collaborators from workflows within the team or enterprise.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.workflows.collaborators.remove/">Slack documentation</a> for more information.</remarks>
    /// <param name="workflowIds">Array of workflow IDs to edit.</param>
    /// <param name="collaboratorIds">Array of collaborators (encoded user IDs) to remove.</param>
    /// <param name="cancellationToken"></param>
    Task Remove(IEnumerable<string> workflowIds, IEnumerable<string> collaboratorIds, CancellationToken cancellationToken = default);
}

public class AdminWorkflowsCollaboratorsApi(ISlackApiClient client) : IAdminWorkflowsCollaboratorsApi
{
    public Task Add(IEnumerable<string> workflowIds, IEnumerable<string> collaboratorIds, CancellationToken cancellationToken = default) =>
        client.Post("admin.workflows.collaborators.add", new Args
            {
                { "workflow_ids", workflowIds },
                { "collaborator_ids", collaboratorIds }
            }, cancellationToken);

    public Task Remove(IEnumerable<string> workflowIds, IEnumerable<string> collaboratorIds, CancellationToken cancellationToken = default) =>
        client.Post("admin.workflows.collaborators.remove", new Args
            {
                { "workflow_ids", workflowIds },
                { "collaborator_ids", collaboratorIds }
            }, cancellationToken);
}
