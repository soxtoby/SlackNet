using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages channels in an Enterprise organization using <a href="https://slack.com/help/articles/360019110974-Slack-Enterprise-Key-Management">Enterprise Key Management</a>.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminConversationsEkmApi
{
    /// <summary>
    /// List all disconnected channels—i.e., channels that were once connected to other workspaces and then disconnected—and
    /// the corresponding original channel IDs for key revocation with EKM.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method only applies to Enterprise organizations using Enterprise Key Management with <a href="https://docs.slack.dev/apis/slack-connect/">Slack Connect</a> channels.
    /// Use this method if you wish to revoke keys on read-only (frozen) channels.
    /// Frozen channels can be created when a channel shared with another organization is subsequently disconnected.
    /// </para>
    /// <para>
    /// When used with <paramref name="teamIds"/>, this method will return all read-only (frozen) channels and their corresponding original channel IDs on the provided workspaces.
    /// When used with <paramref name="channelIds"/>, this method will filter its response, returning the specific original channel IDs requested for the given frozen channels.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.conversations.ekm.listOriginalConnectedChannelInfo/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelIds">The channels to filter to.</param>
    /// <param name="teamIds">The workspaces to which the channels you would like returned belong.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="OriginalConnectedChannelInfoListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<OriginalConnectedChannelInfoListResponse> ListOriginalConnectedChannelInfo(
        IEnumerable<string>? channelIds = null,
        IEnumerable<string>? teamIds = null,
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default);
}

public class AdminConversationsEkmApi(ISlackApiClient client) : IAdminConversationsEkmApi
{
    public Task<OriginalConnectedChannelInfoListResponse> ListOriginalConnectedChannelInfo(
        IEnumerable<string>? channelIds = null,
        IEnumerable<string>? teamIds = null,
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<OriginalConnectedChannelInfoListResponse>("admin.conversations.ekm.listOriginalConnectedChannelInfo", new Args
            {
                { "channel_ids", channelIds },
                { "team_ids", teamIds },
                { "limit", limit },
                { "cursor", cursor }
            }, cancellationToken);
}
