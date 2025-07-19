#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IConversationsRequestSharedInviteApi
{
    /// <summary>
    /// Approves a request to add an external user to a channel and sends them a Slack Connect invite.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/conversations.requestSharedInvite.approve">Slack documentation</a> for more information.</remarks>
    /// <param name="inviteId">ID of the requested shared channel invite to approve.</param>
    /// <param name="isExternalLimited">Optional boolean on whether the invited team will have post-only permissions in the channel. Will override the value on the requested invite.</param>
    /// <param name="channelId">Optional channel ID to which external user will be invited to. Will override the value on the requested invite.</param>
    /// <param name="message">Object describing the text to send along with the invite.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The invite ID.</returns>
    Task<string> Approve(
        string inviteId,
        bool? isExternalLimited = null,
        string? channelId = null,
        ApprovalMessage? message = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Denies a request to invite an external user to a channel.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/conversations.requestSharedInvite.deny">Slack documentation</a> for more information.</remarks>
    /// <param name="inviteId">ID of the requested shared channel invite to deny.</param>
    /// <param name="message">Optional message explaining why the request to invite was denied.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The invite ID.</returns>
    Task<string> Deny(string inviteId, string? message = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists requests to add external users to channels with ability to filter.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/conversations.requestSharedInvite.list">Slack documentation</a> for more information.</remarks>
    /// <param name="userId">Optional filter to return invitation requests for the inviting user.</param>
    /// <param name="includeExpired">When true expired invitation requests will be returned, otherwise they will be excluded.</param>
    /// <param name="includeApproved">When true approved invitation requests will be returned, otherwise they will be excluded.</param>
    /// <param name="includeDenied">When true denied invitation requests will be returned, otherwise they will be excluded.</param>
    /// <param name="inviteIds">An optional list of invitation ids to look up.</param>
    /// <param name="limit">The number of items to return.</param>
    /// <param name="cursor">Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> returned by a previous request's <see cref="InviteRequestListResponse.ResponseMetadata"/>.</param>
    /// <param name="cancellationToken"></param>
    Task<InviteRequestListResponse> List(
        string? userId = null,
        bool includeExpired = false,
        bool includeApproved = false,
        bool includeDenied = false,
        IEnumerable<string>? inviteIds = null,
        int limit = 200,
        string? cursor = null,
        CancellationToken cancellationToken = default);
}

public class ConversationsRequestSharedInviteApi(ISlackApiClient client) : IConversationsRequestSharedInviteApi
{
    public async Task<string> Approve(string inviteId, bool? isExternalLimited = null, string? channelId = null, ApprovalMessage? message = null, CancellationToken cancellationToken = default) =>
        (await client.Post<InviteResponse>("conversations.requestSharedInvite.approve", new Args
            {
                { "invite_id", inviteId },
                { "is_external_limited", isExternalLimited },
                { "channel_id", channelId },
                { "message", message }
            }, cancellationToken).ConfigureAwait(false))
        .InviteId;
    
    public async Task<string> Deny(string inviteId, string? message = null, CancellationToken cancellationToken = default) =>
        (await client.Post<InviteResponse>("conversations.requestSharedInvite.deny", new Args
            {
                { "invite_id", inviteId },
                { "message", message }
            }, cancellationToken).ConfigureAwait(false))
        .InviteId;
    
    public Task<InviteRequestListResponse> List(
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

    class InviteResponse
    {
        public string InviteId { get; set; } = null!;
    }
}