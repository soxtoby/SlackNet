using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages who can access apps in an Enterprise organization, and where they can be used.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminAppsPermissionsApi
{
    /// <summary>
    /// Grant permission for entities to access an app that has its permission type set to <see cref="AppPermissionType.NamedEntities"/>.
    /// Pass either or both <paramref name="userIds"/> and <paramref name="usergroupIds"/> to add those entities to the app's access list.
    /// The app must already have its permission type set to <see cref="AppPermissionType.NamedEntities"/> (see <see cref="Set"/>) before you add entities.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.apps.permissions.add/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">Encoded ID of the app.</param>
    /// <param name="userIds">List of user IDs to allow for <see cref="AppPermissionType.NamedEntities"/> visibility.</param>
    /// <param name="usergroupIds">List of encoded usergroup IDs.</param>
    /// <param name="channelIds">
    /// List of encoded channel IDs to add to the channel restriction list.
    /// Interpretation depends on the app's <see cref="AppPermissions.ChannelRestrictionMode"/>, which is configured via <see cref="Set"/>.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<AppPermissions> Add(
        string appId,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? usergroupIds = null,
        IEnumerable<string>? channelIds = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the permission type of an app and if applicable, includes the entities that have been granted access.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.apps.permissions.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">Encoded ID of the app.</param>
    /// <param name="cancellationToken"></param>
    Task<AppPermissions> List(string appId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke an entity's access to an app that has its permission type set to <see cref="AppPermissionType.NamedEntities"/>,
    /// and remove channels from the app's channel restriction list.
    /// Provide at least one of <paramref name="userIds"/>, <paramref name="usergroupIds"/>, or <paramref name="channelIds"/>.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.apps.permissions.remove/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">Encoded ID of the app.</param>
    /// <param name="userIds">List of user IDs whose <see cref="AppPermissionType.NamedEntities"/> access will be revoked.</param>
    /// <param name="usergroupIds">List of encoded usergroup IDs.</param>
    /// <param name="channelIds">
    /// List of encoded channel IDs to remove from the channel restriction list,
    /// which is either the allowlist under <see cref="ChannelRestrictionMode.SpecificChannels"/> or the exclusion list under <see cref="ChannelRestrictionMode.AllChannelsExcept"/>.
    /// Channels can't be removed while the mode is <see cref="ChannelRestrictionMode.AllChannels"/>.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<AppPermissions> Remove(
        string appId,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? usergroupIds = null,
        IEnumerable<string>? channelIds = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the permission type for who can access an app.
    /// When the <paramref name="permissionType"/> is <see cref="AppPermissionType.NamedEntities"/>,
    /// pass either or both <paramref name="userIds"/> and <paramref name="usergroupIds"/> to define the specific entities that are granted access.
    /// Where <paramref name="permissionType"/> controls which users can access the app, <paramref name="channelRestrictionMode"/> controls which channels it can be used in.
    /// To adjust the channel list later without resending it in full, use <see cref="Add"/> and <see cref="Remove"/>.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.apps.permissions.set/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">Encoded ID of the app.</param>
    /// <param name="permissionType">The type of permission that defines who can access the app.</param>
    /// <param name="userIds">List of user IDs to allow for <see cref="AppPermissionType.NamedEntities"/> visibility.</param>
    /// <param name="usergroupIds">
    /// List of encoded usergroup IDs the permission applies to.
    /// With <see cref="AppPermissionType.NamedEntities"/> the listed groups may access the app; with <see cref="AppPermissionType.NamedEntitiesExclude"/> they may not.
    /// </param>
    /// <param name="channelRestrictionMode">
    /// The mode that defines where the app can be used in channels.
    /// Channel restrictions can't be configured while <paramref name="permissionType"/> is <see cref="AppPermissionType.NoOne"/>, since the app can't be used in any channel.
    /// </param>
    /// <param name="channelIds">
    /// List of encoded channel IDs for channel restrictions.
    /// Semantics depend on <paramref name="channelRestrictionMode"/>:
    /// allowlist for <see cref="ChannelRestrictionMode.SpecificChannels"/>, exclusion list for <see cref="ChannelRestrictionMode.AllChannelsExcept"/>.
    /// Required for those modes, but not for <see cref="ChannelRestrictionMode.AllChannels"/>.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<AppPermissions> Set(
        string appId,
        AppPermissionType permissionType,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? usergroupIds = null,
        ChannelRestrictionMode? channelRestrictionMode = null,
        IEnumerable<string>? channelIds = null,
        CancellationToken cancellationToken = default);
}

public class AdminAppsPermissionsApi(ISlackApiClient client) : IAdminAppsPermissionsApi
{
    public Task<AppPermissions> Add(
        string appId,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? usergroupIds = null,
        IEnumerable<string>? channelIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<AppPermissions>("admin.apps.permissions.add", new Args
            {
                { "app_id", appId },
                { "user_ids", userIds },
                { "usergroup_ids", usergroupIds },
                { "channel_ids", channelIds }
            }, cancellationToken);

    public Task<AppPermissions> List(string appId, CancellationToken cancellationToken = default) =>
        client.Post<AppPermissions>("admin.apps.permissions.list", new Args
            {
                { "app_id", appId }
            }, cancellationToken);

    public Task<AppPermissions> Remove(
        string appId,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? usergroupIds = null,
        IEnumerable<string>? channelIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<AppPermissions>("admin.apps.permissions.remove", new Args
            {
                { "app_id", appId },
                { "user_ids", userIds },
                { "usergroup_ids", usergroupIds },
                { "channel_ids", channelIds }
            }, cancellationToken);

    public Task<AppPermissions> Set(
        string appId,
        AppPermissionType permissionType,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? usergroupIds = null,
        ChannelRestrictionMode? channelRestrictionMode = null,
        IEnumerable<string>? channelIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<AppPermissions>("admin.apps.permissions.set", new Args
            {
                { "app_id", appId },
                { "permission_type", permissionType },
                { "user_ids", userIds },
                { "usergroup_ids", usergroupIds },
                { "channel_restriction_mode", channelRestrictionMode },
                { "channel_ids", channelIds }
            }, cancellationToken);
}
