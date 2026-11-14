#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages public and private channels across an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminConversationsApi
{
    /// <summary>
    /// Archive a public or private channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.archive/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">The channel to archive.</param>
    /// <param name="cancellationToken"></param>
    Task Archive(string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Archive public or private channels in bulk.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.bulkArchive/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelIds">The channels to archive.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The bulk action, with the channels that could not be archived and why.</returns>
    Task<ConversationBulkActionResponse> BulkArchive(IEnumerable<string> channelIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete public or private channels in bulk.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.bulkDelete/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelIds">The channels to delete.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The bulk action, with the channels that could not be deleted and why.</returns>
    Task<ConversationBulkActionResponse> BulkDelete(IEnumerable<string> channelIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Move public or private channels in bulk.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.bulkMove/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelIds">The channels to move.</param>
    /// <param name="targetTeamId">The workspace to move the channels to.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The bulk action, with the channels that could not be moved and why.</returns>
    Task<ConversationBulkActionResponse> BulkMove(IEnumerable<string> channelIds, string targetTeamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclude channels from Slack AI in bulk.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.bulkSetExcludeFromSlackAi/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelIds">The channels to exclude from Slack AI.</param>
    /// <param name="exclude">Whether the channels should be excluded from Slack AI.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The bulk action, with the channels that could not be excluded from Slack AI and why.</returns>
    Task<ConversationBulkActionResponse> BulkSetExcludeFromSlackAi(IEnumerable<string> channelIds, bool exclude, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set a property on channels in bulk.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.bulkSetProperties/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelIds">The channels on which to set the property.</param>
    /// <param name="propertyName">The name of the property to set (e.g. <c>exclude_from_slack_ai</c>).</param>
    /// <param name="propertyValue">The value to set the property to (e.g. <c>true</c>).</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The bulk action, with the channels that could not be updated and why.</returns>
    Task<ConversationBulkUpdateResponse> BulkSetProperties(
        IEnumerable<string> channelIds,
        string propertyName,
        object propertyValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Convert a public channel to a private channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.convertToPrivate/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">The channel to convert to private.</param>
    /// <param name="channelName">Name of private channel to create. Only respected when converting an MPIM.</param>
    /// <param name="cancellationToken"></param>
    Task ConvertToPrivate(string channelId, string? channelName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Convert a private channel to a public channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.convertToPublic/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">The channel to convert to public.</param>
    /// <param name="cancellationToken"></param>
    Task ConvertToPublic(string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a public or private channel-based conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.create/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelName">Name of the public or private channel to create.</param>
    /// <param name="isPrivate">When true, creates a private channel instead of a public channel.</param>
    /// <param name="description">Description of the public or private channel to create.</param>
    /// <param name="orgWide">
    /// When true, the channel will be available org-wide.
    /// Note: if the channel is not org-wide, you must specify a <paramref name="teamId"/> for this channel.
    /// </param>
    /// <param name="teamId">
    /// The workspace to create the channel in.
    /// Note: this argument is required unless you set <paramref name="orgWide"/> to true.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>The ID of the new channel.</returns>
    Task<string> Create(
        string channelName,
        bool isPrivate,
        string? description = null,
        bool orgWide = false,
        string? teamId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a public or private channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.delete/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">The channel to delete.</param>
    /// <param name="cancellationToken"></param>
    Task Delete(string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnect a connected channel from one or more workspaces.
    /// </summary>
    /// <remarks>
    /// Note that the originating workspace (i.e., the workspace where a channel originally was created) may not be disconnected.
    /// For a channel connected to other organizations, only the originating organization may call this method.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.disconnectShared/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">The channel to be disconnected from some workspaces.</param>
    /// <param name="leavingTeamIds">The workspaces to disconnect from the shared channel.</param>
    /// <param name="cancellationToken"></param>
    Task DisconnectShared(string channelId, IEnumerable<string>? leavingTeamIds = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all the workspaces a given public or private channel is connected to within this Enterprise org.
    /// </summary>
    /// <remarks>
    /// This method doesn't list workspaces from other Enterprise organizations that are also connected to the same channel.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.getTeams/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">The channel to determine connected workspaces within the organization for.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="ConversationTeamsResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="cancellationToken"></param>
    Task<ConversationTeamsResponse> GetTeams(
        string channelId,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invite users to a public or private channel.
    /// </summary>
    /// <remarks>
    /// If the method fails for some of the users, a <see cref="SlackException"/> is thrown with a <c>failed_for_some_users</c> error.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.invite/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">The channel that the users will be invited to.</param>
    /// <param name="userIds">The users to invite.</param>
    /// <param name="cancellationToken"></param>
    Task Invite(string channelId, IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove multiple users from a conversation.
    /// Admin version of <see cref="IConversationsApi.Kick"/> with support for bulk user removal.
    /// </summary>
    /// <remarks>
    /// If none of the users could be removed, a <see cref="SlackException"/> is thrown with a <c>failed_for_all_users</c> error.
    /// Read the users that could not be removed with <see cref="SlackException.ErrorField{T}"/> using <c>failed_user_ids</c>,
    /// as a list of <see cref="ConversationKickFailure"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.kick/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">ID of conversation to remove users from.</param>
    /// <param name="userIds">The users to remove from the channel.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The users that were removed, and the users that could not be removed, with why.</returns>
    Task<ConversationKickResponse> Kick(string channelId, IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns channels on the given team using the filters.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.lookup/">Slack documentation</a> for more information.</remarks>
    /// <param name="teamIds">Team IDs to filter by.</param>
    /// <param name="lastMessageActivityBefore">Filter by public channels where the most recent message was sent before this time.</param>
    /// <param name="maxMemberCount">Filter by public channels with member count equal to or less than the specified number.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="ConversationLookupResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">Maximum number of results.</param>
    /// <param name="cancellationToken"></param>
    Task<ConversationLookupResponse> Lookup(
        IEnumerable<string> teamIds,
        DateTime lastMessageActivityBefore,
        int? maxMemberCount = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Rename a public or private channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.rename/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">The channel to rename.</param>
    /// <param name="channelName">The new name for the channel.</param>
    /// <param name="cancellationToken"></param>
    Task Rename(string channelId, string channelName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search for public or private channels in an Enterprise organization.
    /// </summary>
    /// <remarks>
    /// Searching private channels with this method will only work if the user associated with the user token being used
    /// has permission to manage private channels on the org.
    /// By default, only the <a href="https://slack.com/help/articles/360052445454-Manage-permissions-for-channel-management-tools#h_01J6FQ9ATH7WVCZCZCHKG9P64Z">primary owner of the org has that level of permission</a>.
    /// Missing this permission is a cause of the <c>not_allowed</c> error.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.search/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamIds">The internal workspaces to search through.</param>
    /// <param name="connectedTeamIds">The external orgs to search through.</param>
    /// <param name="query">Name of the channel to query by.</param>
    /// <param name="limit">Maximum number of items to be returned. Default is 10.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to the <see cref="AdminConversationSearchResponse.NextCursor"/>
    /// returned by a previous request.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="searchChannelTypes">
    /// The types of channel to include or exclude in the search.
    /// For example <see cref="ConversationSearchType.Private"/> will search private channels, while <see cref="ConversationSearchType.PrivateExclude"/> will exclude them.
    /// If you pass multiple types, the search occurs in channels that satisfy any of the types.
    /// </param>
    /// <param name="sort">How to sort the channels. Defaults to <see cref="ConversationSearchSort.MemberCount"/>.</param>
    /// <param name="sortDirection">Sort direction. Defaults to <see cref="SortDirection.Descending"/>.</param>
    /// <param name="totalCountOnly">
    /// Only return the <see cref="AdminConversationSearchResponse.TotalCount"/> of channels.
    /// Omits channel data and allows access for admins without channel manager permissions.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<AdminConversationSearchResponse> Search(
        IEnumerable<string>? teamIds = null,
        IEnumerable<string>? connectedTeamIds = null,
        string? query = null,
        int? limit = null,
        string? cursor = null,
        IEnumerable<ConversationSearchType>? searchChannelTypes = null,
        ConversationSearchSort? sort = null,
        SortDirection? sortDirection = null,
        bool totalCountOnly = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the workspaces in an Enterprise org that connect to a public or private channel.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When used with <paramref name="teamId"/>, this method sets the channel as a regular channel on that workspace.
    /// When used with <paramref name="targetTeamIds"/>, this method sets the channel as a
    /// <a href="https://docs.slack.dev/apis/slack-connect/">cross-workspace shared channel</a>,
    /// shared to all the workspaces in <paramref name="targetTeamIds"/>.
    /// Either way, this method can be used both to add and to remove workspaces from a channel, including the workspace that originated the channel.
    /// </para>
    /// <para>
    /// When connecting a workspace to many channels at the same time, the message history for each channel will be available after all channels have been processed.
    /// To access a channel's message history before the process is complete, a hard refresh is required.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.setTeams/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">The channel to add or remove to workspaces.</param>
    /// <param name="teamId">
    /// The workspace to which the channel belongs if the channel is a local workspace channel.
    /// Omit this argument if the channel is a cross-workspace or org-wide shared channel.
    /// </param>
    /// <param name="targetTeamIds">The workspaces to which the channel should be shared. Not required if the channel is being shared org-wide.</param>
    /// <param name="orgChannel">True if channel has to be converted to an org channel.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The ID of the channel, or null if Slack doesn't return it.</returns>
    Task<string?> SetTeams(
        string channelId,
        string? teamId = null,
        IEnumerable<string>? targetTeamIds = null,
        bool orgChannel = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unarchive a public or private channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.unarchive/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">The channel to unarchive.</param>
    /// <param name="cancellationToken"></param>
    Task Unarchive(string channelId, CancellationToken cancellationToken = default);
}

public class AdminConversationsApi(ISlackApiClient client) : IAdminConversationsApi
{
    public Task Archive(string channelId, CancellationToken cancellationToken = default) =>
        client.Get("admin.conversations.archive", new Args { { "channel_id", channelId } }, cancellationToken);

    public Task<ConversationBulkActionResponse> BulkArchive(IEnumerable<string> channelIds, CancellationToken cancellationToken = default) =>
        client.Get<ConversationBulkActionResponse>("admin.conversations.bulkArchive", new Args { { "channel_ids", channelIds } }, cancellationToken);

    public Task<ConversationBulkActionResponse> BulkDelete(IEnumerable<string> channelIds, CancellationToken cancellationToken = default) =>
        client.Get<ConversationBulkActionResponse>("admin.conversations.bulkDelete", new Args { { "channel_ids", channelIds } }, cancellationToken);

    public Task<ConversationBulkActionResponse> BulkMove(IEnumerable<string> channelIds, string targetTeamId, CancellationToken cancellationToken = default) =>
        client.Get<ConversationBulkActionResponse>("admin.conversations.bulkMove", new Args
            {
                { "channel_ids", channelIds },
                { "target_team_id", targetTeamId }
            }, cancellationToken);

    public Task<ConversationBulkActionResponse> BulkSetExcludeFromSlackAi(IEnumerable<string> channelIds, bool exclude, CancellationToken cancellationToken = default) =>
        client.Get<ConversationBulkActionResponse>("admin.conversations.bulkSetExcludeFromSlackAi", new Args
            {
                { "channel_ids", channelIds },
                { "exclude", exclude }
            }, cancellationToken);

    public Task<ConversationBulkUpdateResponse> BulkSetProperties(
        IEnumerable<string> channelIds,
        string propertyName,
        object propertyValue,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<ConversationBulkUpdateResponse>("admin.conversations.bulkSetProperties", new Args
            {
                { "channel_ids", channelIds },
                { "property", new Dictionary<string, object> { { propertyName, propertyValue } } }
            }, cancellationToken);

    public Task ConvertToPrivate(string channelId, string? channelName = null, CancellationToken cancellationToken = default) =>
        client.Get("admin.conversations.convertToPrivate", new Args
            {
                { "channel_id", channelId },
                { "name", channelName }
            }, cancellationToken);

    public Task ConvertToPublic(string channelId, CancellationToken cancellationToken = default) =>
        client.Get("admin.conversations.convertToPublic", new Args { { "channel_id", channelId } }, cancellationToken);

    public async Task<string> Create(
        string channelName,
        bool isPrivate,
        string? description = null,
        bool orgWide = false,
        string? teamId = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Get<ChannelIdResponse>("admin.conversations.create", new Args
            {
                { "name", channelName },
                { "is_private", isPrivate },
                { "description", description },
                { "org_wide", orgWide },
                { "team_id", teamId }
            }, cancellationToken).ConfigureAwait(false)).ChannelId;

    public Task Delete(string channelId, CancellationToken cancellationToken = default) =>
        client.Get("admin.conversations.delete", new Args { { "channel_id", channelId } }, cancellationToken);

    public Task DisconnectShared(string channelId, IEnumerable<string>? leavingTeamIds = null, CancellationToken cancellationToken = default) =>
        client.Get("admin.conversations.disconnectShared", new Args
            {
                { "channel_id", channelId },
                { "leaving_team_ids", leavingTeamIds }
            }, cancellationToken);

    public Task<ConversationTeamsResponse> GetTeams(
        string channelId,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<ConversationTeamsResponse>("admin.conversations.getTeams", new Args
            {
                { "channel_id", channelId },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);

    public Task Invite(string channelId, IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
        client.Get("admin.conversations.invite", new Args
            {
                { "channel_id", channelId },
                { "user_ids", userIds }
            }, cancellationToken);

    public Task<ConversationKickResponse> Kick(string channelId, IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
        client.Get<ConversationKickResponse>("admin.conversations.kick", new Args
            {
                { "channel_id", channelId },
                { "user_ids", userIds }
            }, cancellationToken);

    public Task<ConversationLookupResponse> Lookup(
        IEnumerable<string> teamIds,
        DateTime lastMessageActivityBefore,
        int? maxMemberCount = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<ConversationLookupResponse>("admin.conversations.lookup", new Args
            {
                { "team_ids", teamIds },
                { "last_message_activity_before", lastMessageActivityBefore.ToTimestampNumber() },
                { "max_member_count", maxMemberCount },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);

    public Task Rename(string channelId, string channelName, CancellationToken cancellationToken = default) =>
        client.Get("admin.conversations.rename", new Args
            {
                { "channel_id", channelId },
                { "name", channelName }
            }, cancellationToken);

    public Task<AdminConversationSearchResponse> Search(
        IEnumerable<string>? teamIds = null,
        IEnumerable<string>? connectedTeamIds = null,
        string? query = null,
        int? limit = null,
        string? cursor = null,
        IEnumerable<ConversationSearchType>? searchChannelTypes = null,
        ConversationSearchSort? sort = null,
        SortDirection? sortDirection = null,
        bool totalCountOnly = false,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<AdminConversationSearchResponse>("admin.conversations.search", new Args
            {
                { "team_ids", teamIds },
                { "connected_team_ids", connectedTeamIds },
                { "query", query },
                { "limit", limit },
                { "cursor", cursor },
                { "search_channel_types", searchChannelTypes },
                { "sort", sort },
                { "sort_dir", sortDirection },
                { "total_count_only", totalCountOnly }
            }, cancellationToken);

    public async Task<string?> SetTeams(
        string channelId,
        string? teamId = null,
        IEnumerable<string>? targetTeamIds = null,
        bool orgChannel = false,
        CancellationToken cancellationToken = default
    ) =>
        // Slack's example response has no channel, which leaves no response object to read it from
        (await client.Get<ChannelResponse>("admin.conversations.setTeams", new Args
            {
                { "channel_id", channelId },
                { "team_id", teamId },
                { "target_team_ids", targetTeamIds },
                { "org_channel", orgChannel }
            }, cancellationToken).ConfigureAwait(false))?.Channel;

    public Task Unarchive(string channelId, CancellationToken cancellationToken = default) =>
        client.Get("admin.conversations.unarchive", new Args { { "channel_id", channelId } }, cancellationToken);
}
