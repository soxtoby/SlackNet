﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IConversationsApi
{
#nullable enable
    /// <summary>
    /// Approves a request to add an external user to a channel and sends them a Slack Connect invite.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.requestSharedInvite.approve/">Slack documentation</a> for more information.</remarks>
    /// <param name="inviteId">ID of the requested shared channel invite to approve.</param>
    /// <param name="isExternalLimited">Optional boolean on whether the invited team will have post-only permissions in the channel. Will override the value on the requested invite.</param>
    /// <param name="channelId">Optional channel ID to which external user will be invited to. Will override the value on the requested invite.</param>
    /// <param name="message">Object describing the text to send along with the invite.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The invite ID.</returns>
    Task<string> ApproveRequestedShareInvite(
        string inviteId,
        bool? isExternalLimited = null,
        string? channelId = null,
        ApprovalMessage? message = null,
        CancellationToken cancellationToken = default);
#nullable disable
    
    /// <summary>
    /// Accepts an invitation to a Slack Connect channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.acceptSharedInvite/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelName">Name of the channel. If the channel does not exist already in your workspace, this name is the one that the channel will take.</param>
    /// <param name="inviteId"><see cref="SlackNet.Invite.Id"/> from the <see cref="SharedChannelInviteReceived"/> event.</param>
    /// <param name="channelId">ID of the channel that you'd like to accept. Must provide either <see cref="inviteId"/> or <see cref="channelId"/>.</param>
    /// <param name="freeTrialAccepted">Whether you'd like to use your workspace's free trial to begin using Slack Connect.</param>
    /// <param name="isPrivate">Whether the channel should be private.</param>
    /// <param name="teamId">The ID of the workspace to accept the channel in. If an org-level token is used to call this method, the <see cref="teamId"/> argument is required.</param>
    /// <param name="cancellationToken"></param>
    Task<AcceptSharedInviteResponse> AcceptSharedInvite(string channelName, string inviteId = null, string channelId = null, bool freeTrialAccepted = false, bool isPrivate = false, string teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Approves an invitation to a Slack Connect channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.approveSharedInvite/">Slack documentation</a> for more information.</remarks>
    /// <param name="inviteId">ID of the shared channel invite to approve. Subscribe to the <see cref="SharedChannelInviteAccepted"/> event to receive IDs of Slack Connect channel invites that have been accepted and are awaiting approval.</param>
    /// <param name="targetTeam">The team or enterprise id of the other party involved in the invitation you are approving.</param>
    /// <param name="cancellationToken"></param>
    Task ApproveSharedInvite(string inviteId, string targetTeam = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Archives a conversation. Not all types of conversations can be archived.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.archive/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">ID of conversation to archive.</param>
    /// <param name="cancellationToken"></param>
    Task Archive(string channelId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a channel canvas for a channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.canvases.create/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel ID of the channel the canvas will be tabbed in.</param>
    /// <param name="documentContent">Structure describing the type and value of the content to create.</param>
    /// <param name="title">Title of the newly created canvas.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The ID of the newly created canvas.</returns>
    Task<string> CreateCanvas(string channelId, [CanBeNull] DocumentContent documentContent = null, [CanBeNull] string title = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes a direct message or multi-person direct message.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.close/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Conversation to close.</param>
    /// <param name="cancellationToken"></param>
    Task Close(string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates a public or private channel-based conversation.
    /// Use <c>Open</c> to initiate or resume a direct message or multi-person direct message.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.create/">Slack documentation</a> for more information.</remarks>
    /// <param name="name">Name of the public or private channel to create.</param>
    /// <param name="isPrivate">Create a private channel instead of a public one.</param>
    /// <param name="teamId">Encoded team id to create the channel in, required if org token is used</param>
    /// <param name="cancellationToken"></param>
    Task<Conversation> Create(string name, bool isPrivate, string teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Declines a Slack Connect channel invite.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.declineSharedInvite/">Slack documentation</a> for more information.</remarks>
    /// <param name="inviteId">ID of the Slack Connect invite to decline. Subscribe to the <see cref="SharedChannelInviteAccepted"/> event to receive IDs of Slack Connect channel invites that have been accepted and are awaiting approval.</param>
    /// <param name="targetTeam">The team or enterprise id of the other party involved in the invitation you are declining.</param>
    /// <param name="cancellationToken"></param>
    Task DeclineSharedInvite(string inviteId, string targetTeam = null, CancellationToken cancellationToken = default);

#nullable enable
    /// <summary>
    /// Denies a request to invite an external user to a channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.requestSharedInvite.deny/">Slack documentation</a> for more information.</remarks>
    /// <param name="inviteId">ID of the requested shared channel invite to deny.</param>
    /// <param name="message">Optional message explaining why the request to invite was denied.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The invite ID.</returns>
    Task<string> DenyRequestedShareInvite(string inviteId, string? message = null, CancellationToken cancellationToken = default);
#nullable disable

    /// <summary>
    /// Fetches a conversation's history of messages and events.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.history/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Conversation ID to fetch history for.</param>
    /// <param name="latestTs">End of time range of messages to include in results.</param>
    /// <param name="oldestTs">Start of time range of messages to include in results.</param>
    /// <param name="inclusive">Include messages with latest or oldest timestamp in results only when either timestamp is specified.</param>
    /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
    /// <param name="includeAllMetadata">Return all metadata associated with the messages.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="ConversationMessagesResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<ConversationHistoryResponse> History(string channelId, string latestTs = null, string oldestTs = null, bool inclusive = false, int limit = 100, bool includeAllMetadata = false, string cursor = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve information about a conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.info/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Conversation ID to learn more about.</param>
    /// <param name="includeLocale">Set this to true to receive the locale for this conversation.</param>
    /// <param name="includeNumMembers"></param>
    /// <param name="cancellationToken"></param>
    Task<Conversation> Info(string channelId, bool includeLocale = false, bool includeNumMembers = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invites users to a channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.invite/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">The ID of the public or private channel to invite user(s) to.</param>
    /// <param name="userIds">A comma separated list of user IDs. Up to 30 users may be listed.</param>
    /// <param name="cancellationToken"></param>
    Task<Conversation> Invite(string channelId, IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an invitation to a Slack Connect channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.inviteShared/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">ID of the channel on your team that you'd like to share.</param>
    /// <param name="emails">Emails to receive this invite. Either <see cref="emails"/> or <see cref="userIds"/> must be provided.</param>
    /// <param name="userIds">User IDs to receive this invite. Either <see cref="emails"/> or <see cref="userIds"/> must be provided.</param>
    /// <param name="externalLimited">Whether invite is to an external limited member.</param>
    /// <param name="cancellationToken"></param>
    Task<InviteSharedResponse> InviteShared(string channelId, IEnumerable<string> emails, IEnumerable<string> userIds, bool externalLimited = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Joins an existing conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.join/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">ID of conversation to join.</param>
    /// <param name="cancellationToken"></param>
    Task<ConversationJoinResponse> Join(string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a user from a conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.kick/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">ID of conversation to remove user from.</param>
    /// <param name="userId">User ID to be removed.</param>
    /// <param name="cancellationToken"></param>
    Task Kick(string channelId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Leaves a conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.leave">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Conversation to leave.</param>
    /// <param name="cancellationToken"></param>
    Task Leave(string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all channels in a Slack team.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.list">Slack documentation</a> for more information.</remarks>
    /// <param name="excludeArchived">Set to True to exclude archived channels from the list.</param>
    /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the list hasn't been reached. Must be an integer no larger than 1000.</param>
    /// <param name="types">Types of conversations to include. Default is <see cref="ConversationType.PublicChannel"/>.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="ConversationListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="teamId">encoded team id to list channels in, required if token belongs to org-wide app</param>
    /// <param name="cancellationToken"></param>
    Task<ConversationListResponse> List(bool excludeArchived = false, int limit = 100, IEnumerable<ConversationType> types = null, string cursor = null, string teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists shared channel invites that have been generated or received but have not been approved by all parties.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.listConnectInvites">Slack documentation</a> for more information.</remarks>
    /// <param name="count">Maximum number of invites to return.</param>
    /// <param name="cursor">Set to <see cref="ResponseMetadata.NextCursor"/> returned by previous call to list items in subsequent page.</param>
    /// <param name="teamId">Encoded team id for the workspace to retrieve invites for, required if org token is used.</param>
    /// <param name="cancellationToken"></param>
    Task<ConnectInvitesListResponse> ListConnectInvites(int count = 100, string cursor = null, string teamId = null, CancellationToken cancellationToken = default);
    
#nullable enable
    /// <summary>
    /// Lists requests to add external users to channels with ability to filter.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.requestSharedInvite.list">Slack documentation</a> for more information.</remarks>
    /// <param name="userId">Optional filter to return invitation requests for the inviting user.</param>
    /// <param name="includeExpired">When true expired invitation requests will be returned, otherwise they will be excluded.</param>
    /// <param name="includeApproved">When true approved invitation requests will be returned, otherwise they will be excluded.</param>
    /// <param name="includeDenied">When true denied invitation requests will be returned, otherwise they will be excluded.</param>
    /// <param name="inviteIds">An optional list of invitation ids to look up.</param>
    /// <param name="limit">The number of items to return.</param>
    /// <param name="cursor">Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> returned by a previous request's <see cref="InviteRequestListResponse.ResponseMetadata"/>.</param>
    /// <param name="cancellationToken"></param>
    Task<InviteRequestListResponse> ListRequestedShareInvites(
        string? userId = null,
        bool includeExpired = false,
        bool includeApproved = false,
        bool includeDenied = false,
        IEnumerable<string>? inviteIds = null,
        int limit = 200,
        string? cursor = null,
        CancellationToken cancellationToken = default);
#nullable disable

    /// <summary>
    /// Sets the read cursor in a channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.mark">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel or conversation to set the read cursor for.</param>
    /// <param name="messageTs">Unique identifier of message you want marked as most recently seen in this conversation.</param>
    /// <param name="cancellationToken"></param>
    Task Mark(string channelId, string messageTs, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve members of a conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.members">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">ID of the conversation to retrieve members for.</param>
    /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="ConversationMembersResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<ConversationMembersResponse> Members(string channelId, int limit = 100, string cursor = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens or resumes a direct message or multi-person direct message.
    /// Use <see cref="Create"/> for public or private channels.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.open">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Resume a conversation by supplying an im or mpim's ID.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The opened channel's ID</returns>
    Task<string> Open(string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens or resumes a direct message or multi-person direct message.
    /// Use <see cref="Create"/> for public or private channels.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.open">Slack documentation</a> for more information.</remarks>
    /// <param name="userIds">List of users. If only one user is included, this creates a 1:1 DM. The ordering of the users is preserved whenever a multi-person direct message is returned.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The opened channel's ID</returns>
    Task<string> Open(IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens or resumes a direct message or multi-person direct message.
    /// Use <see cref="Create"/> for public or private channels.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.open">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Resume a conversation by supplying an im or mpim's ID.</param>
    /// <param name="cancellationToken"></param>
    Task<ConversationOpenResponse> OpenAndReturnInfo(string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens or resumes a direct message or multi-person direct message.
    /// Use <see cref="Create"/> for public or private channels.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.open">Slack documentation</a> for more information.</remarks>
    /// <param name="userIds">List of users. If only one user is included, this creates a 1:1 DM. The ordering of the users is preserved whenever a multi-person direct message is returned.</param>
    /// <param name="cancellationToken"></param>
    Task<ConversationOpenResponse> OpenAndReturnInfo(IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renames a conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.rename">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">ID of conversation to rename.</param>
    /// <param name="name">New name for conversation.</param>
    /// <param name="cancellationToken"></param>
    Task<Conversation> Rename(string channelId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve a thread of messages posted to a conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.replies">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Conversation ID to fetch thread from.</param>
    /// <param name="threadTs">Unique identifier of a thread's parent message.</param>
    /// <param name="latestTs">End of time range of messages to include in results.</param>
    /// <param name="oldestTs">Start of time range of messages to include in results.</param>
    /// <param name="inclusive">Include messages with latest or oldest timestamp in results only when either timestamp is specified.</param>
    /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
    /// <param name="includeAllMetadata">Return all metadata associated with this message.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="ConversationMessagesResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<ConversationMessagesResponse> Replies(
        string channelId,
        string threadTs,
        string latestTs = null,
        string oldestTs = null,
        bool inclusive = false,
        int limit = 10,
        bool includeAllMetadata = false,
        string cursor = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Upgrade or downgrade Slack Connect channel permissions between 'can post only' and 'can post and invite'.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.externalInvitePermissions.set">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">The channel ID to change external invite permissions for.</param>
    /// <param name="targetTeamId">The team ID of the target team. Must be in the specified channel.</param>
    /// <param name="action">Type of action to be taken: upgrade or downgrade.</param>
    /// <param name="cancellationToken"></param>
    Task SetExternalInvitePermissions(string channelId, string targetTeamId, PermissionAction action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the purpose for a conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.setPurpose">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Conversation to set the purpose of.</param>
    /// <param name="purpose">A new, specialer purpose.</param>
    /// <param name="cancellationToken"></param>
    Task<string> SetPurpose(string channelId, string purpose, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the topic for a conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.setTopic">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Conversation to set the topic of.</param>
    /// <param name="topic">The new topic string. Does not support formatting or linkification.</param>
    /// <param name="cancellationToken"></param>
    Task<string> SetTopic(string channelId, string topic, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reverses conversation archival.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/conversations.unarchive">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">ID of conversation to unarchive.</param>
    /// <param name="cancellationToken"></param>
    Task Unarchive(string channelId, CancellationToken cancellationToken = default);
}

public class ConversationsApi(ISlackApiClient client) : IConversationsApi
{
#nullable enable
    public async Task<string> ApproveRequestedShareInvite(string inviteId, bool? isExternalLimited = null, string? channelId = null, ApprovalMessage? message = null, CancellationToken cancellationToken = default) =>
        (await client.Post<InviteResponse>("conversations.requestSharedInvite.approve", new Args
            {
                { "invite_id", inviteId },
                { "is_external_limited", isExternalLimited },
                { "channel_id", channelId },
                { "message", message }
            }, cancellationToken).ConfigureAwait(false))
        .InviteId;
#nullable disable
    
    public Task<AcceptSharedInviteResponse> AcceptSharedInvite(string channelName, string inviteId = null, string channelId = null, bool freeTrialAccepted = false, bool isPrivate = false, string teamId = null, CancellationToken cancellationToken = default) =>
        client.Post<AcceptSharedInviteResponse>("conversations.acceptSharedInvite", new Args
            {
                { "channel_name", channelName },
                { "invite_id", inviteId },
                { "channel_id", channelId },
                { "free_trial_accepted", freeTrialAccepted },
                { "is_private", isPrivate },
                { "team_id", teamId }
            }, cancellationToken);

    public Task ApproveSharedInvite(string inviteId, string targetTeam = null, CancellationToken cancellationToken = default) =>
        client.Post("conversations.approveSharedInvite", new Args
            {
                { "invite_id", inviteId },
                { "target_team", targetTeam }
            }, cancellationToken);

    public Task Archive(string channelId, CancellationToken cancellationToken = default) =>
        client.Post("conversations.archive", new Args { { "channel", channelId } }, cancellationToken);

    public async Task<string> CreateCanvas(string channelId, DocumentContent documentContent = null, string title = null, CancellationToken cancellationToken = default) =>
        (await client.Post<CanvasCreationResponse>("conversations.canvases.create",
            new Args
                {
                    { "channel_id", channelId },
                    { "document_content", documentContent },
                    { "title", title }
                },
            cancellationToken).ConfigureAwait(false))
        .CanvasId;

    public Task Close(string channelId, CancellationToken cancellationToken = default) =>
        client.Post("conversations.close", new Args { { "channel", channelId } }, cancellationToken);

    public async Task<Conversation> Create(string name, bool isPrivate, string teamId = null, CancellationToken cancellationToken = default) =>
        (await client.Post<ConversationResponse>("conversations.create", new Args
            {
                { "name", name },
                { "is_private", isPrivate },
                { "team_id", teamId }
            }, cancellationToken).ConfigureAwait(false))
        .Channel;

    public Task DeclineSharedInvite(string inviteId, string targetTeam = null, CancellationToken cancellationToken = default) =>
        client.Get("conversations.declineSharedInvite", new Args
            {
                { "invite_id", inviteId },
                { "target_team", targetTeam }
            }, cancellationToken);
    
#nullable enable
    public async Task<string> DenyRequestedShareInvite(string inviteId, string? message = null, CancellationToken cancellationToken = default) =>
        (await client.Post<InviteResponse>("conversations.requestSharedInvite.deny", new Args
            {
                { "invite_id", inviteId },
                { "message", message }
            }, cancellationToken).ConfigureAwait(false))
        .InviteId;
#nullable disable

    public Task<ConversationHistoryResponse> History(string channelId, string latestTs = null, string oldestTs = null, bool inclusive = false, int limit = 100, bool includeAllMetadata = false, string cursor = null, CancellationToken cancellationToken = default) =>
        client.Get<ConversationHistoryResponse>("conversations.history", new Args
            {
                { "channel", channelId },
                { "cursor", cursor },
                { "inclusive", inclusive },
                { "latest", latestTs },
                { "limit", limit },
                { "oldest", oldestTs },
                { "include_all_metadata", includeAllMetadata }
            }, cancellationToken);

    public async Task<Conversation> Info(string channelId, bool includeLocale = false, bool includeNumMembers = false, CancellationToken cancellationToken = default) =>
        (await client.Get<ConversationResponse>("conversations.info", new Args
            {
                { "channel", channelId },
                { "include_locale", includeLocale },
                { "include_num_members", includeNumMembers }
            }, cancellationToken).ConfigureAwait(false))
        .Channel;

    public async Task<Conversation> Invite(string channelId, IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
        (await client.Post<ConversationResponse>("conversations.invite", new Args
            {
                { "channel", channelId },
                { "users", userIds }
            }, cancellationToken).ConfigureAwait(false))
        .Channel;

    public Task<InviteSharedResponse> InviteShared(string channelId, IEnumerable<string> emails, IEnumerable<string> userIds, bool externalLimited = true, CancellationToken cancellationToken = default) =>
        client.Get<InviteSharedResponse>("conversations.inviteShared", new Args()
            {
                { "channel", channelId },
                { "emails", emails },
                { "user_ids", userIds },
                { "external_limited", externalLimited }
            }, cancellationToken);

    public Task<ConversationJoinResponse> Join(string channelId, CancellationToken cancellationToken = default) =>
        client.Post<ConversationJoinResponse>("conversations.join", new Args { { "channel", channelId } }, cancellationToken);

    public Task Kick(string channelId, string userId, CancellationToken cancellationToken = default) =>
        client.Post("conversations.kick", new Args
            {
                { "channel", channelId },
                { "user", userId }
            }, cancellationToken);

    public Task Leave(string channelId, CancellationToken cancellationToken = default) =>
        client.Post("conversations.leave", new Args { { "channel", channelId } }, cancellationToken);

    public Task<ConversationListResponse> List(bool excludeArchived = false, int limit = 100, IEnumerable<ConversationType> types = null, string cursor = null, string teamId = null, CancellationToken cancellationToken = default) =>
        client.Get<ConversationListResponse>("conversations.list", new Args
            {
                { "cursor", cursor },
                { "exclude_archived", excludeArchived },
                { "limit", limit },
                { "types", types },
                { "team_id", teamId }
            }, cancellationToken);

    public Task<ConnectInvitesListResponse> ListConnectInvites(int count = 100, string cursor = null, string teamId = null, CancellationToken cancellationToken = default) =>
        client.Post<ConnectInvitesListResponse>("conversations.listConnectInvites", new Args
            {
                { "count", count },
                { "cursor", cursor },
                { "team_id", teamId }
            }, cancellationToken);
    
#nullable enable
    public Task<InviteRequestListResponse> ListRequestedShareInvites(
        string? userId = null,
        bool includeExpired = false,
        bool includeApproved = false,
        bool includeDenied = false,
        IEnumerable<string>? inviteIds = null,
        int limit = 200,
        string? cursor = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<InviteRequestListResponse>("conversations.requestSharedInvite.list", new Args
            {
                { "user_id", userId },
                { "include_expired", includeExpired },
                { "include_approved", includeApproved },
                { "include_denied", includeDenied },
                { "invite_ids", inviteIds },
                { "limit", limit },
                { "cursor", cursor }
            }, cancellationToken);
#nullable disable

    public Task Mark(string channelId, string messageTs, CancellationToken cancellationToken = default) =>
        client.Post("conversations.mark", new Args
            {
                { "channel", channelId },
                { "ts", messageTs }
            }, cancellationToken);

    public Task<ConversationMembersResponse> Members(string channelId, int limit = 100, string cursor = null, CancellationToken cancellationToken = default) =>
        client.Get<ConversationMembersResponse>("conversations.members", new Args
            {
                { "channel", channelId },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);

    public async Task<string> Open(string channelId, CancellationToken cancellationToken = default) =>
        (await Open<ConversationIdResponse>(false, channelId, null, cancellationToken).ConfigureAwait(false)).Channel.Id;

    public async Task<string> Open(IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
        (await Open<ConversationIdResponse>(false, null, userIds, cancellationToken).ConfigureAwait(false)).Channel.Id;

    public Task<ConversationOpenResponse> OpenAndReturnInfo(string channelId, CancellationToken cancellationToken = default) =>
        Open<ConversationOpenResponse>(true, channelId, null, cancellationToken);

    public Task<ConversationOpenResponse> OpenAndReturnInfo(IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
        Open<ConversationOpenResponse>(true, null, userIds, cancellationToken);

    private Task<T> Open<T>(bool returnIm, string channelId = null, IEnumerable<string> userIds = null, CancellationToken cancellationToken = default) where T : class =>
        client.Post<T>("conversations.open", new Args
            {
                { "channel", channelId },
                { "return_im", returnIm },
                { "users", userIds }
            }, cancellationToken);

    public async Task<Conversation> Rename(string channelId, string name, CancellationToken cancellationToken = default) =>
        (await client.Post<ConversationResponse>("conversations.rename", new Args
            {
                { "channel", channelId },
                { "name", name }
            }, cancellationToken).ConfigureAwait(false))
        .Channel;

    public Task<ConversationMessagesResponse> Replies(
        string channelId,
        string threadTs,
        string latestTs = null,
        string oldestTs = null,
        bool inclusive = false,
        int limit = 10,
        bool includeAllMetadata = false,
        string cursor = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<ConversationMessagesResponse>("conversations.replies", new Args
            {
                { "channel", channelId },
                { "ts", threadTs },
                { "cursor", cursor },
                { "inclusive", inclusive },
                { "latest", latestTs },
                { "limit", limit },
                { "include_all_metadata", includeAllMetadata },
                { "oldest", oldestTs }
            }, cancellationToken);

    public Task SetExternalInvitePermissions(string channelId, string targetTeamId, PermissionAction action, CancellationToken cancellationToken = default) =>
        client.Post("conversations.externalInvitePermissions.set", new Args
            {
                { "channel", channelId },
                { "target_team", targetTeamId },
                { "action", action }
            }, cancellationToken);

    public async Task<string> SetPurpose(string channelId, string purpose, CancellationToken cancellationToken = default) =>
        (await client.Post<PurposeResponse>("conversations.setPurpose", new Args
            {
                { "channel", channelId },
                { "purpose", purpose }
            }, cancellationToken).ConfigureAwait(false))
        .Purpose;

    public async Task<string> SetTopic(string channelId, string topic, CancellationToken cancellationToken = default) =>
        (await client.Post<TopicResponse>("conversations.setTopic", new Args
            {
                { "channel", channelId },
                { "topic", topic }
            }, cancellationToken).ConfigureAwait(false))
        .Topic;

    public Task Unarchive(string channelId, CancellationToken cancellationToken = default) =>
        client.Post("conversations.unarchive", new Args { { "channel", channelId } }, cancellationToken);
}