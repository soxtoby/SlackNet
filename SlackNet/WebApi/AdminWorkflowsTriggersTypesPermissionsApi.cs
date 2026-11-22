using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages who can use each trigger type in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminWorkflowsTriggersTypesPermissionsApi
{
    /// <summary>
    /// List the permissions for using each trigger type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Depending on a toggle, each trigger type's permissions are returned either as a <see cref="TriggerTypePermissions.Permission"/>
    /// with the entities it applies to, or as granular <see cref="TriggerTypePermissions.Permissions"/>.
    /// Check whether <see cref="TriggerTypePermissions.Permissions"/> is null to determine which format is being used.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.workflows.triggers.types.permissions.lookup/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="triggerTypeIds">The trigger type IDs for which to get the permissions.</param>
    /// <param name="cancellationToken"></param>
    Task<TriggerTypePermissionsLookupResponse> Lookup(IEnumerable<string> triggerTypeIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the permissions for using a trigger type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use either <paramref name="visibility"/> or <paramref name="permissions"/> to configure permissions, but not both.
    /// When using <see cref="PermissionVisibility.NamedEntities"/> visibility, provide at least one of
    /// <paramref name="userIds"/>, <paramref name="teamIds"/> or <paramref name="orgIds"/>,
    /// or their equivalents in each <see cref="TriggerTypePermissionSetting"/>.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.workflows.triggers.types.permissions.set/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="triggerTypeId">The trigger type ID for which to set the permissions.</param>
    /// <param name="visibility">The trigger type visibility.</param>
    /// <param name="userIds">List of user IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.</param>
    /// <param name="teamIds">List of workspace IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.</param>
    /// <param name="orgIds">List of enterprise IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.</param>
    /// <param name="permissions">Permissions for multiple permission types.</param>
    /// <param name="cancellationToken"></param>
    Task Set(
        string triggerTypeId,
        PermissionVisibility? visibility = null,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? teamIds = null,
        IEnumerable<string>? orgIds = null,
        IEnumerable<TriggerTypePermissionSetting>? permissions = null,
        CancellationToken cancellationToken = default);
}

public class AdminWorkflowsTriggersTypesPermissionsApi(ISlackApiClient client) : IAdminWorkflowsTriggersTypesPermissionsApi
{
    public Task<TriggerTypePermissionsLookupResponse> Lookup(IEnumerable<string> triggerTypeIds, CancellationToken cancellationToken = default) =>
        client.Post<TriggerTypePermissionsLookupResponse>("admin.workflows.triggers.types.permissions.lookup", new Args
            {
                { "trigger_type_ids", triggerTypeIds }
            }, cancellationToken);

    public Task Set(
        string triggerTypeId,
        PermissionVisibility? visibility = null,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? teamIds = null,
        IEnumerable<string>? orgIds = null,
        IEnumerable<TriggerTypePermissionSetting>? permissions = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("admin.workflows.triggers.types.permissions.set", new Args
            {
                { "id", triggerTypeId },
                { "visibility", visibility },
                { "user_ids", userIds },
                { "team_ids", teamIds },
                { "org_ids", orgIds },
                { "permissions", permissions }
            }, cancellationToken);
}
