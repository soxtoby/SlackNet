#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Searches and unpublishes workflows in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminWorkflowsApi
{
    /// <summary>
    /// Search workflows within the team or enterprise.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method allows searching across all workflows in the top-level team of the caller.
    /// It returns an array of all workflows in the org, letting you filter by app ID, name, etc.
    /// </para>
    /// <para>
    /// Since workflows can have an unlimited number of triggers, there may be some cases where Slack is not able to accurately report trigger information in the response.
    /// For example, if a workflow has hundreds of triggers, <see cref="Workflow.TriggerTypes"/> and <see cref="Workflow.TriggerIds"/> are only populated for a subset of those triggers.
    /// Use <paramref name="numTriggerIds"/> to fetch more triggers.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.workflows.search/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="query">A search query to filter for workflow name or description.</param>
    /// <param name="appId">The parent app ID for which to return workflows.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="WorkflowSearchResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The maximum number of items to return. Defaults to 20.</param>
    /// <param name="noCollaborators">Only include workflows with no collaborators in the result.</param>
    /// <param name="collaboratorIds">Only include workflows where all of the provided user IDs are a manager/collaborator of that workflow.</param>
    /// <param name="numTriggerIds">Number of trigger IDs to fetch for each workflow. Defaults to 10.</param>
    /// <param name="isSalesElevate">Filter workflows by their Sales Elevate status.</param>
    /// <param name="source">Source of workflow creation, either from code or workflow builder.</param>
    /// <param name="sort">The field used to sort the returned workflows. Defaults to <see cref="WorkflowSearchSort.Name"/>.</param>
    /// <param name="sortDirection">Sort direction. Defaults to <see cref="SortDirection.Descending"/>.</param>
    /// <param name="triggerTypeId">Only include workflows with this trigger type.</param>
    /// <param name="publishStatus">Filter workflows by their published status. Defaults to <see cref="PublishStatus.Published"/>.</param>
    /// <param name="stepFunctionIds">Only include workflows that use all of the provided step function IDs.</param>
    /// <param name="cancellationToken"></param>
    Task<WorkflowSearchResponse> Search(
        string? query = null,
        string? appId = null,
        string? cursor = null,
        int? limit = null,
        bool noCollaborators = false,
        IEnumerable<string>? collaboratorIds = null,
        int? numTriggerIds = null,
        bool? isSalesElevate = null,
        WorkflowSource? source = null,
        WorkflowSearchSort? sort = null,
        SortDirection? sortDirection = null,
        string? triggerTypeId = null,
        PublishStatus? publishStatus = null,
        IEnumerable<string>? stepFunctionIds = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unpublish workflows within the team or enterprise.
    /// </summary>
    /// <remarks>
    /// This method unpublishes a set of workflows built in Workflow Builder.
    /// Because coded workflows cannot be unpublished, the underlying app would need to be uninstalled instead (see <see cref="IAdminAppsApi.Uninstall"/>).
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.workflows.unpublish/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="workflowIds">Array of workflow IDs to unpublish.</param>
    /// <param name="cancellationToken"></param>
    Task Unpublish(IEnumerable<string> workflowIds, CancellationToken cancellationToken = default);
}

public class AdminWorkflowsApi(ISlackApiClient client) : IAdminWorkflowsApi
{
    public Task<WorkflowSearchResponse> Search(
        string? query = null,
        string? appId = null,
        string? cursor = null,
        int? limit = null,
        bool noCollaborators = false,
        IEnumerable<string>? collaboratorIds = null,
        int? numTriggerIds = null,
        bool? isSalesElevate = null,
        WorkflowSource? source = null,
        WorkflowSearchSort? sort = null,
        SortDirection? sortDirection = null,
        string? triggerTypeId = null,
        PublishStatus? publishStatus = null,
        IEnumerable<string>? stepFunctionIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<WorkflowSearchResponse>("admin.workflows.search", new Args
            {
                { "query", query },
                { "app_id", appId },
                { "cursor", cursor },
                { "limit", limit },
                { "no_collaborators", noCollaborators },
                { "collaborator_ids", collaboratorIds },
                { "num_trigger_ids", numTriggerIds },
                { "is_sales_elevate", isSalesElevate },
                { "source", source },
                { "sort", sort },
                { "sort_dir", sortDirection },
                { "trigger_type_id", triggerTypeId },
                { "publish_status", publishStatus },
                { "step_function_ids", stepFunctionIds }
            }, cancellationToken);

    public Task Unpublish(IEnumerable<string> workflowIds, CancellationToken cancellationToken = default) =>
        client.Post("admin.workflows.unpublish", new Args
            {
                { "workflow_ids", workflowIds }
            }, cancellationToken);
}
