#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Looks up who can run workflows in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminWorkflowsPermissionsApi
{
    /// <summary>
    /// Look up the permissions for a set of workflows.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method looks up permissions for which users are authorized to run workflows in the org.
    /// Since workflows themselves do not have an execution policy, the rule exists on the trigger associated with the workflow.
    /// For workflows created in Workflow Builder, there is a 1:1 mapping with a trigger.
    /// For coded apps, Slack merges the permissions of all of a workflow's triggers and returns the most permissive set.
    /// If there are numerous triggers, Slack will only check a subset of them (determined by <paramref name="maxWorkflowTriggers"/>)
    /// and return false for <see cref="WorkflowPermissions.Complete"/> to denote this.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.workflows.permissions.lookup/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="workflowIds">An array of workflow IDs to look up permissions for.</param>
    /// <param name="maxWorkflowTriggers">Maximum number of triggers to fetch for each workflow when determining overall run permissions. Defaults to 100.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The workflows' permissions, keyed by workflow ID.</returns>
    Task<IDictionary<string, WorkflowPermissions>> Lookup(
        IEnumerable<string> workflowIds,
        int? maxWorkflowTriggers = null,
        CancellationToken cancellationToken = default);
}

public class AdminWorkflowsPermissionsApi(ISlackApiClient client) : IAdminWorkflowsPermissionsApi
{
    public async Task<IDictionary<string, WorkflowPermissions>> Lookup(
        IEnumerable<string> workflowIds,
        int? maxWorkflowTriggers = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<WorkflowPermissionsLookupResponse>("admin.workflows.permissions.lookup", new Args
            {
                { "workflow_ids", workflowIds },
                { "max_workflow_triggers", maxWorkflowTriggers }
            }, cancellationToken).ConfigureAwait(false)).Permissions;
}
