#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IListAccessApi
{
    /// <summary>
    /// Set the access level to a List for specified channels.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.access.set/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">Encoded ID of the List.</param>
    /// <param name="accessLevel">Desired level of access.</param>
    /// <param name="channelIds">List of channels you wish to update access for.</param>
    /// <param name="cancellationToken"></param>
    Task SetChannelAccessLevel(
        string listId,
        AccessLevel accessLevel,
        IEnumerable<string> channelIds,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Set the access level to a List for specified users.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.access.set/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">Encoded ID of the List.</param>
    /// <param name="accessLevel">Desired level of access.</param>
    /// <param name="userIds">List of users you wish to update access for.</param>
    /// <param name="cancellationToken"></param>
    Task SetUserAccessLevel(
        string listId,
        AccessLevel accessLevel,
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Revoke access to a List for specified channels.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.access.delete/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">Encoded ID of the List.</param>
    /// <param name="channelIds">List of channels you wish to update access for.</param>
    /// <param name="cancellationToken"></param>
    Task DeleteChannelAccess(
        string listId,
        IEnumerable<string> channelIds,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Revoke access to a List for specified users.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.access.delete/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">Encoded ID of the List.</param>
    /// <param name="userIds">List of users you wish to update access for.</param>
    /// <param name="cancellationToken"></param>
    Task DeleteUserAccess(
        string listId,
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default
    );
}

public class ListAccessApi(ISlackApiClient client) : IListAccessApi
{
    public Task SetChannelAccessLevel(
        string listId,
        AccessLevel accessLevel,
        IEnumerable<string>? channelIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("slackLists.access.set",
            new Args
                {
                    { "list_id", listId },
                    { "access_level", accessLevel },
                    { "channel_ids", channelIds }
                },
            cancellationToken);

    public Task SetUserAccessLevel(
        string listId,
        AccessLevel accessLevel,
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("slackLists.access.set",
            new Args
                {
                    { "list_id", listId },
                    { "access_level", accessLevel },
                    { "user_ids", userIds }
                },
            cancellationToken);

    public Task DeleteChannelAccess(
        string listId,
        IEnumerable<string> channelIds,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("slackLists.access.delete",
            new Args
                {
                    { "list_id", listId },
                    { "channel_ids", channelIds }
                },
            cancellationToken);

    public Task DeleteUserAccess(
        string listId,
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("slackLists.access.delete",
            new Args
                {
                    { "list_id", listId },
                    { "user_ids", userIds }
                },
            cancellationToken);
}