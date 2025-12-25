using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IWorkflowTriggerPermissionsApi
{
    /// <summary>
    /// Allows users to run a trigger that has its permission type set to <see cref="TriggerPermissionType.NamedEntities"/>.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/workflows.triggers.permissions.add">Slack documentation</a> for more information.</remarks>
    /// <param name="triggerId">Encoded ID of the trigger.</param>
    /// <param name="userIds">List of encoded user IDs.</param>
    /// <param name="channelIds">List of encoded channel IDs.</param>
    /// <param name="teamIds">List of encoded workspace IDs.</param>
    /// <param name="orgIds">List of encoded organization IDs.</param>
    /// <param name="cancellationToken"></param>
    Task<TriggerPermissionsResponse> Add(
        string triggerId,
        IEnumerable<string> userIds = null,
        IEnumerable<string> channelIds = null,
        IEnumerable<string> teamIds = null,
        IEnumerable<string> orgIds = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Returns the permission type of a trigger and if applicable, includes the entities that have been granted access.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/workflows.triggers.permissions.list">Slack documentation</a> for more information.</remarks>
    /// <param name="triggerId">Encoded ID of the trigger.</param>
    /// <param name="cancellationToken"></param>
    Task<TriggerPermissionsResponse> List(string triggerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke an entity's access to a trigger that has its permission type set to <see cref="TriggerPermissionType.NamedEntities"/>.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/workflows.triggers.permissions.remove">Slack documentation</a> for more information.</remarks>
    /// <param name="triggerId">Encoded ID of the trigger.</param>
    /// <param name="userIds">List of encoded user IDs.</param>
    /// <param name="channelIds">List of encoded channel IDs.</param>
    /// <param name="teamIds">List of encoded workspace IDs.</param>
    /// <param name="orgIds">List of encoded organization IDs.</param>
    /// <param name="cancellationToken"></param>
    Task<TriggerPermissionsResponse> Remove(
        string triggerId,
        IEnumerable<string> userIds = null,
        IEnumerable<string> channelIds = null,
        IEnumerable<string> teamIds = null,
        IEnumerable<string> orgIds = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Set the permission type for who can run a trigger.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/workflows.triggers.permissions.set">Slack documentation</a> for more information.</remarks>
    /// <param name="triggerId">Encoded ID of the trigger.</param>
    /// <param name="permissionType">The permission type for who can run a trigger.</param>
    /// <param name="userIds">List of encoded user IDs.</param>
    /// <param name="channelIds">List of encoded channel IDs.</param>
    /// <param name="teamIds">List of encoded workspace IDs.</param>
    /// <param name="orgIds">List of encoded organization IDs.</param>
    /// <param name="cancellationToken"></param>
    Task<TriggerPermissionsResponse> Set(
        string triggerId,
        TriggerPermissionType permissionType,
        IEnumerable<string> userIds = null,
        IEnumerable<string> channelIds = null,
        IEnumerable<string> teamIds = null,
        IEnumerable<string> orgIds = null,
        CancellationToken cancellationToken = default
    );
}

public class WorkflowTriggerPermissionsApi(ISlackApiClient client) : IWorkflowTriggerPermissionsApi
{
    public Task<TriggerPermissionsResponse> Add(
        string triggerId,
        IEnumerable<string> userIds = null,
        IEnumerable<string> channelIds = null,
        IEnumerable<string> teamIds = null,
        IEnumerable<string> orgIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<TriggerPermissionsResponse>("workflows.triggers.permissions.add", new Args
            {
                { "trigger_id", triggerId },
                { "user_ids", userIds },
                { "channel_ids", channelIds },
                { "team_ids", teamIds },
                { "org_ids", orgIds }
            }, cancellationToken);

    public Task<TriggerPermissionsResponse> List(string triggerId, CancellationToken cancellationToken = default) =>
        client.Post<TriggerPermissionsResponse>("workflows.triggers.permissions.list", new Args
            {
                { "trigger_id", triggerId }
            }, cancellationToken);

    public Task<TriggerPermissionsResponse> Remove(
        string triggerId,
        IEnumerable<string> userIds = null,
        IEnumerable<string> channelIds = null,
        IEnumerable<string> teamIds = null,
        IEnumerable<string> orgIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<TriggerPermissionsResponse>("workflows.triggers.permissions.remove", new Args
            {
                { "trigger_id", triggerId },
                { "user_ids", userIds },
                { "channel_ids", channelIds },
                { "team_ids", teamIds },
                { "org_ids", orgIds }
            }, cancellationToken);

    public Task<TriggerPermissionsResponse> Set(
        string triggerId,
        TriggerPermissionType permissionType,
        IEnumerable<string> userIds = null,
        IEnumerable<string> channelIds = null,
        IEnumerable<string> teamIds = null,
        IEnumerable<string> orgIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<TriggerPermissionsResponse>("workflows.triggers.permissions.set", new Args
            {
                { "trigger_id", triggerId },
                { "permission_type", permissionType },
                { "user_ids", userIds },
                { "channel_ids", channelIds },
                { "team_ids", teamIds },
                { "org_ids", orgIds }
            }, cancellationToken);
}
