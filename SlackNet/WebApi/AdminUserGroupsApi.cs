#nullable enable
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages organization-wide user groups, including IDP groups and their default channels and workspaces, in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminUserGroupsApi
{
    /// <summary>
    /// Add default channels to an IDP group.
    /// </summary>
    /// <remarks>
    /// <para>Default org-wide channels (where everyone is a member anyway) <b>cannot</b> be set as a default for an IDP group with this method.</para>
    /// <para>
    /// If a channel specified has already been linked to the given IDP group, no changes occur
    /// (for example, users that have chosen to leave the default channel will not be added again).
    /// </para>
    /// <para>
    /// If only some of the channels are added, a <see cref="SlackException"/> is thrown with a <c>channel_not_found</c> error.
    /// Read the IDs of the channels that could not be added with <see cref="SlackException.ErrorField{T}"/> using <c>invalid_channels</c>.
    /// You might see a partial success if you do not have enough permissions to manage all the specified channels,
    /// if some channel IDs are not valid, or if the types of some channels are not supported.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.usergroups.addChannels/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userGroupId">ID of the IDP group to add default channels for.</param>
    /// <param name="channelIds">Channel IDs.</param>
    /// <param name="teamId">The workspace to add default channels in.</param>
    /// <param name="cancellationToken"></param>
    Task AddChannels(string userGroupId, IEnumerable<string> channelIds, string? teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Associate one or more default workspaces with an organization-wide IDP group.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this method to add workspaces to an organization-wide
    /// <a href="https://slack.com/help/articles/115001435788-Connect-identity-provider-groups-to-your-Enterprise-organization">IDP group</a>.
    /// When you link workspaces to an IDP group, you automatically add members of the IDP group to those workspaces.
    /// </para>
    /// <para>
    /// If an error occurs on one or more workspaces, a <see cref="SlackException"/> is thrown with an <c>auto_provision_failure</c> error.
    /// If one or more team IDs provided were invalid, it is thrown with an <c>invalid_team_ids</c> error.
    /// Read the IDs of the workspaces that were <i>not</i> successfully linked to the IDP group with <see cref="SlackException.ErrorField{T}"/> using <c>errors</c>.
    /// Note that some workspaces may successfully be added while others fail.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.usergroups.addTeams/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userGroupId">An encoded usergroup (IDP group) ID.</param>
    /// <param name="teamIds">Encoded team (workspace) IDs. Each workspace <i>must</i> belong to the organization associated with the token.</param>
    /// <param name="autoProvision">When <c>true</c>, this method automatically creates new workspace accounts for the IDP group members.</param>
    /// <param name="cancellationToken"></param>
    Task AddTeams(string userGroupId, IEnumerable<string> teamIds, bool autoProvision = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add members to an existing organizational usergroup.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.usergroups.addUsers/">Slack documentation</a> for more information.</remarks>
    /// <param name="userGroupId">ID of the usergroup to add users to.</param>
    /// <param name="userIds">The encoded user IDs to add to the usergroup.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Any users that could not be added.</returns>
    Task<IList<UserGroupUserFailure>> AddUsers(string userGroupId, IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new organizational usergroup.
    /// </summary>
    /// <remarks>
    /// This method creates an organization-wide usergroup that can then have workspaces, channels, and members associated with it.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.usergroups.create/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userGroupName">Unique name for the usergroup.</param>
    /// <param name="handle">Handle used to mention the usergroup in a channel. Must be unique.</param>
    /// <param name="purpose">Purpose that describes what the usergroup is about.</param>
    /// <param name="isVisible">Configure whether or not this usergroup should be visible in the client.</param>
    /// <param name="cancellationToken"></param>
    Task<UserGroup> Create(
        string userGroupName,
        string? handle = null,
        string? purpose = null,
        bool? isVisible = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetch an organizational usergroup.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.usergroups.fetch/">Slack documentation</a> for more information.</remarks>
    /// <param name="userGroupId">ID of the usergroup to fetch.</param>
    /// <param name="cancellationToken"></param>
    Task<UserGroup> Fetch(string userGroupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// List the channels linked to an org-level IDP group (user group).
    /// </summary>
    /// <remarks>
    /// This method's response is limited to the channels that the requesting token
    /// (and thus, the installing <a href="https://slack.com/help/articles/360018112273-Roles-in-Slack#enterprise-grid-plan-1">Org Admin or Owner</a>) can see.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.usergroups.listChannels/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userGroupId">ID of the IDP group to list default channels for.</param>
    /// <param name="teamId">ID of the workspace.</param>
    /// <param name="includeNumMembers">Include the count of members per channel, as <see cref="Conversation.NumMembers"/>.</param>
    /// <param name="cancellationToken"></param>
    Task<IList<Conversation>> ListChannels(
        string userGroupId,
        string? teamId = null,
        bool includeNumMembers = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove one or more default channels from an org-level IDP group (user group).
    /// </summary>
    /// <remarks>
    /// <para>
    /// If only some of the channels are removed, a <see cref="SlackException"/> is thrown with a <c>channel_not_found</c> error.
    /// Read the IDs of the channels that could not be removed with <see cref="SlackException.ErrorField{T}"/> using <c>invalid_channels</c>.
    /// You might see a partial success if you do not have enough permissions to manage all the specified channels,
    /// if some channel IDs are not valid, or if the types of some channels are not supported.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.usergroups.removeChannels/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userGroupId">ID of the IDP group.</param>
    /// <param name="channelIds">Channel IDs.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveChannels(string userGroupId, IEnumerable<string> channelIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove one or more default workspaces from an organization-wide IDP group or Admin group.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If one or more team IDs provided were invalid, a <see cref="SlackException"/> is thrown with an <c>invalid_team_ids</c> error.
    /// Read the invalid team IDs with <see cref="SlackException.ErrorField{T}"/> using <c>errors</c>.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.usergroups.removeTeams/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userGroupId">An encoded usergroup (IDP group) ID.</param>
    /// <param name="teamIds">Encoded team (workspace) IDs. Each workspace <i>must</i> belong to the organization associated with the token.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveTeams(string userGroupId, IEnumerable<string> teamIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove members from an existing organizational usergroup.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.usergroups.removeUsers/">Slack documentation</a> for more information.</remarks>
    /// <param name="userGroupId">ID of the usergroup to remove users from.</param>
    /// <param name="userIds">The encoded user IDs to remove from the usergroup.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveUsers(string userGroupId, IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update one or more properties of an existing organizational usergroup.
    /// </summary>
    /// <remarks>
    /// <para>Provide only the properties you want to change; omitted properties are left as they are.</para>
    /// <para>
    /// If the update would cause one or more members to exceed their channel limit, a <see cref="SlackException"/> is thrown with a <c>user_channel_limit_exceeded</c> error.
    /// Read the IDs of the users who would exceed their channel limit with <see cref="SlackException.ErrorField{T}"/> using <c>channel_limit_user_ids</c>.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.usergroups.update/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userGroupId">ID of the usergroup to update.</param>
    /// <param name="userGroupName">The name of the group.</param>
    /// <param name="handle">The handle used for mentioning the group in a channel. Must be unique.</param>
    /// <param name="description">The usergroup's purpose or description.</param>
    /// <param name="isVisible">Configure whether or not this usergroup should be visible in the client.</param>
    /// <param name="cancellationToken"></param>
    Task<UserGroup> Update(
        string userGroupId,
        string? userGroupName = null,
        string? handle = null,
        string? description = null,
        bool? isVisible = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add members to an existing organizational usergroup in bulk via CSV upload.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.usergroups.uploadUsers/">Slack documentation</a> for more information.</remarks>
    /// <param name="userGroupId">ID of the usergroup to upload users to.</param>
    /// <param name="file">
    /// CSV of users to upload, in the format <c>member id, email</c>.
    /// Only its <see cref="FileUpload.FileName"/> and content are used.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<UserGroupUploadUsersResponse> UploadUsers(string userGroupId, FileUpload file, CancellationToken cancellationToken = default);
}

public class AdminUserGroupsApi(ISlackApiClient client) : IAdminUserGroupsApi
{
    public Task AddChannels(string userGroupId, IEnumerable<string> channelIds, string? teamId = null, CancellationToken cancellationToken = default) =>
        client.Post("admin.usergroups.addChannels", new Args
            {
                { "usergroup_id", userGroupId },
                { "channel_ids", channelIds },
                { "team_id", teamId }
            }, cancellationToken);

    public Task AddTeams(string userGroupId, IEnumerable<string> teamIds, bool autoProvision = false, CancellationToken cancellationToken = default) =>
        client.Post("admin.usergroups.addTeams", new Args
            {
                { "usergroup_id", userGroupId },
                { "team_ids", teamIds },
                { "auto_provision", autoProvision }
            }, cancellationToken);

    public async Task<IList<UserGroupUserFailure>> AddUsers(string userGroupId, IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
        (await client.Post<UserGroupAddUsersResponse>("admin.usergroups.addUsers", new Args
            {
                { "id", userGroupId },
                { "users", userIds }
            }, cancellationToken).ConfigureAwait(false))
        .InvalidUsers;

    public async Task<UserGroup> Create(
        string userGroupName,
        string? handle = null,
        string? purpose = null,
        bool? isVisible = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<SubteamResponse>("admin.usergroups.create", new Args
            {
                { "name", userGroupName },
                { "handle", handle },
                { "purpose", purpose },
                { "is_visible", isVisible }
            }, cancellationToken).ConfigureAwait(false))
        .Subteam;

    public async Task<UserGroup> Fetch(string userGroupId, CancellationToken cancellationToken = default) =>
        (await client.Post<SubteamResponse>("admin.usergroups.fetch", new Args { { "id", userGroupId } }, cancellationToken).ConfigureAwait(false))
        .Subteam;

    public async Task<IList<Conversation>> ListChannels(
        string userGroupId,
        string? teamId = null,
        bool includeNumMembers = false,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<ConversationListResponse>("admin.usergroups.listChannels", new Args
            {
                { "usergroup_id", userGroupId },
                { "team_id", teamId },
                { "include_num_members", includeNumMembers }
            }, cancellationToken).ConfigureAwait(false))
        .Channels;

    public Task RemoveChannels(string userGroupId, IEnumerable<string> channelIds, CancellationToken cancellationToken = default) =>
        client.Post("admin.usergroups.removeChannels", new Args
            {
                { "usergroup_id", userGroupId },
                { "channel_ids", channelIds }
            }, cancellationToken);

    public Task RemoveTeams(string userGroupId, IEnumerable<string> teamIds, CancellationToken cancellationToken = default) =>
        client.Post("admin.usergroups.removeTeams", new Args
            {
                { "usergroup_id", userGroupId },
                { "team_ids", teamIds }
            }, cancellationToken);

    public Task RemoveUsers(string userGroupId, IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
        client.Post("admin.usergroups.removeUsers", new Args
            {
                { "id", userGroupId },
                { "users", userIds }
            }, cancellationToken);

    public async Task<UserGroup> Update(
        string userGroupId,
        string? userGroupName = null,
        string? handle = null,
        string? description = null,
        bool? isVisible = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<SubteamResponse>("admin.usergroups.update", new Args
            {
                { "id", userGroupId },
                { "name", userGroupName },
                { "handle", handle },
                { "description", description },
                { "is_visible", isVisible }
            }, cancellationToken).ConfigureAwait(false))
        .Subteam;

    public async Task<UserGroupUploadUsersResponse> UploadUsers(string userGroupId, FileUpload file, CancellationToken cancellationToken = default)
    {
        using var content = file.GetContent();
        return await client.Post<UserGroupUploadUsersResponse>("admin.usergroups.uploadUsers", new Args { { "id", userGroupId } }, new MultipartFormDataContent
                {
                    { content.HttpContent, "file", file.FileName }
                },
            cancellationToken).ConfigureAwait(false);
    }
}
