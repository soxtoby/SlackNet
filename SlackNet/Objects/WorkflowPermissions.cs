#nullable enable
using SlackNet.WebApi;

namespace SlackNet;

/// <summary>
/// Who can run a workflow, as returned by <see cref="IAdminWorkflowsPermissionsApi.Lookup"/>.
/// </summary>
public class WorkflowPermissions
{
    /// <summary>
    /// The most permissive set of the permissions of the workflow's triggers.
    /// </summary>
    public TriggerPermissionsResponse WhoCanRun { get; set; } = new();

    /// <summary>
    /// False when the check for all workflow triggers was not exhaustive and there may be additional entities that can run the workflow.
    /// </summary>
    public bool Complete { get; set; }
}
