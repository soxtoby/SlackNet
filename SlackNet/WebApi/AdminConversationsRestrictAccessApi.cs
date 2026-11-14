#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages the <a href="https://slack.com/help/articles/115001435788-Connect-identity-provider-groups-to-your-Enterprise-organization">IDP groups</a>
/// that are allowed to access private channels in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminConversationsRestrictAccessApi
{
    /// <summary>
    /// Add an allowlist of IDP groups for accessing a channel.
    /// </summary>
    /// <remarks>
    /// Adds an IDP group to a private channel's allowlist, so that only members belonging to the IDP group can access the channel.
    /// If the added IDP group is the first group linked to this channel, any user who is not a member of that group is removed from the channel,
    /// including bot users from integrations or apps.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.restrictAccess.addGroup/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">The channel to link this group to.</param>
    /// <param name="usergroupId">The IDP Group ID to be an allowlist for the private channel.</param>
    /// <param name="teamId">
    /// The workspace where the channel exists.
    /// This argument is required for channels only tied to one workspace, and optional for channels that are shared across an organization.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task AddGroup(string channelId, string usergroupId, string? teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// List all IDP Groups linked to a channel.
    /// </summary>
    /// <remarks>
    /// Lists IDP groups that are allowed access to a private channel. Only members of these IDP groups may access the channel.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.restrictAccess.listGroups/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">The channel to list the linked groups of.</param>
    /// <param name="teamId">
    /// The workspace where the channel exists.
    /// This argument is required for channels only tied to one workspace, and optional for channels that are shared across an organization.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>The IDs of the IDP groups linked to the channel.</returns>
    Task<IList<string>> ListGroups(string channelId, string? teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a linked IDP group from a private channel.
    /// </summary>
    /// <remarks>
    /// Removes an IDP group from a private channel's allowlist.
    /// Any members that were only in this IDP group and none of the other linked IDP groups for that channel will be removed from the channel as well.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.restrictAccess.removeGroup/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">The channel to remove the linked group from.</param>
    /// <param name="usergroupId">The IDP Group ID to remove from the private channel.</param>
    /// <param name="teamId">The workspace where the channel exists.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveGroup(string channelId, string usergroupId, string teamId, CancellationToken cancellationToken = default);
}

public class AdminConversationsRestrictAccessApi(ISlackApiClient client) : IAdminConversationsRestrictAccessApi
{
    public Task AddGroup(string channelId, string usergroupId, string? teamId = null, CancellationToken cancellationToken = default) =>
        client.Get("admin.conversations.restrictAccess.addGroup", new Args
            {
                { "channel_id", channelId },
                { "group_id", usergroupId },
                { "team_id", teamId }
            }, cancellationToken);

    public async Task<IList<string>> ListGroups(string channelId, string? teamId = null, CancellationToken cancellationToken = default) =>
        (await client.Get<GroupIdsResponse>("admin.conversations.restrictAccess.listGroups", new Args
            {
                { "channel_id", channelId },
                { "team_id", teamId }
            }, cancellationToken).ConfigureAwait(false)).GroupIds;

    public Task RemoveGroup(string channelId, string usergroupId, string teamId, CancellationToken cancellationToken = default) =>
        client.Get("admin.conversations.restrictAccess.removeGroup", new Args
            {
                { "channel_id", channelId },
                { "group_id", usergroupId },
                { "team_id", teamId }
            }, cancellationToken);
}
