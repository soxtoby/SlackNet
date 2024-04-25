using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface ICallsApi
{
    /// <summary>
    /// Registers a new Call.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/calls.add">Slack documentation</a> for more information.</remarks>
    /// <param name="externalUniqueId">An ID supplied by the 3rd-party Call provider. It must be unique across all Calls from that service.</param>
    /// <param name="joinUrl">The URL required for a client to join the Call.</param>
    /// <param name="createdByUserId">The valid Slack user ID of the user who created this Call. When this method is called with a user token, the <see cref="createdByUserId"/> parameter is optional and defaults to the authed user of the token. Otherwise, the field is required.</param>
    /// <param name="dateStart">Call start time.</param>
    /// <param name="desktopAppJoinUrl">When supplied, available Slack clients will attempt to directly launch the 3rd-party Call with this URL.</param>
    /// <param name="externalDisplayId">An optional, human-readable ID supplied by the 3rd-party Call provider. If supplied, this ID will be displayed in the Call object.</param>
    /// <param name="title">The name of the Call.</param>
    /// <param name="users">The list of users to register as participants in the Call.</param>
    /// <param name="cancellationToken"></param>
    Task<Call> Add(
        string externalUniqueId,
        string joinUrl,
        string createdByUserId = null,
        DateTime? dateStart = null,
        string desktopAppJoinUrl = null,
        string externalDisplayId = null,
        string title = null,
        IEnumerable<CallUser> users = null,
        CancellationToken? cancellationToken = null);

    /// <summary>
    /// Ends a Call.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/calls.end">Slack documentation</a> for more information.</remarks>
    /// <param name="id"><see cref="Call.Id"/> returned when registering the call using the <see cref="Add"/> method.</param>
    /// <param name="duration">Call duration.</param>
    /// <param name="cancellationToken"></param>
    Task End(string id, TimeSpan? duration = null, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Returns information about a Call.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/calls.info">Slack documentation</a> for more information.</remarks>
    /// <param name="id"><see cref="Call.Id"/> returned by the <see cref="Add"/> method.</param>
    /// <param name="cancellationToken"></param>
    Task<Call> Info(string id, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Updates information about a Call.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/calls.update">Slack documentation</a> for more information.</remarks>
    /// <param name="id"><see cref="Call.Id"/> returned by the <see cref="Add"/> method.</param>
    /// <param name="desktopAppJoinUrl">When supplied, available Slack clients will attempt to directly launch the 3rd-party Call with this URL.</param>
    /// <param name="joinUrl">The URL required for a client to join the Call.</param>
    /// <param name="title">The name of the Call.</param>
    /// <param name="cancellationToken"></param>
    Task<Call> Update(string id, string desktopAppJoinUrl = null, string joinUrl = null, string title = null, CancellationToken? cancellationToken = null);
}

public class CallsApi(ISlackApiClient client) : ICallsApi
{
    public async Task<Call> Add(
        string externalUniqueId,
        string joinUrl,
        string createdByUserId = null,
        DateTime? dateStart = null,
        string desktopAppJoinUrl = null,
        string externalDisplayId = null,
        string title = null,
        IEnumerable<CallUser> users = null,
        CancellationToken? cancellationToken = null
    ) =>
        (await client.Post<CallResponse>("calls.add", new Args
            {
                { "external_unique_id", externalUniqueId },
                { "join_url", joinUrl },
                { "created_by", createdByUserId },
                { "date_start", dateStart?.ToTimestamp() },
                { "desktop_app_join_url", desktopAppJoinUrl },
                { "external_display_id", externalDisplayId },
                { "title", title },
                { "users", users }
            }, cancellationToken).ConfigureAwait(false))
        .Call;

    public Task End(string id, TimeSpan? duration = null, CancellationToken? cancellationToken = null) =>
        client.Post("calls.end", new Args
            {
                { "id", id },
                { "duration", duration?.TotalSeconds }
            }, cancellationToken);

    public async Task<Call> Info(string id, CancellationToken? cancellationToken = null) =>
        (await client.Post<CallResponse>("calls.info", new Args { { "id", id } }, cancellationToken).ConfigureAwait(false)).Call;

    public async Task<Call> Update(string id, string desktopAppJoinUrl = null, string joinUrl = null, string title = null, CancellationToken? cancellationToken = null) =>
        (await client.Post<CallResponse>("calls.update", new Args
            {
                { "id", id },
                { "desktop_app_join_url", desktopAppJoinUrl },
                { "join_url", joinUrl },
                { "title", title }
            }, cancellationToken).ConfigureAwait(false))
        .Call;
}