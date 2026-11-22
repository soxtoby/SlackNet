using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Pauses and resumes organization-wide access for frontline members of an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminUsersAccessApi
{
    /// <summary>
    /// Pause access across the organization for frontline members.
    /// Success means the request was queued, not that all updates have completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Targets must be frontline members in the authenticated organization. Valid users are queued even if other targets are invalid.
    /// If some users are rejected, a <see cref="SlackException"/> is thrown with a <c>failed_for_some_users</c> error.
    /// When none are valid, it is thrown with a <c>failed_for_all_users</c> error, and nothing is queued.
    /// Read the rejected users with <see cref="SlackException.ErrorField{T}"/> using <c>invalid_users</c>, as a list of <see cref="UserFailure"/>.
    /// For a partially accepted request, correct and retry only the rejected IDs.
    /// Authentication and queueing failures return an error without queueing any users.
    /// </para>
    /// <para>
    /// A successful response means the request was queued.
    /// Updates happen asynchronously, and failures during job execution are not returned in this response.
    /// Pausing an already-paused member preserves the original pause timestamp. Sessions are not invalidated.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.access.pause/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userIds">IDs of frontline members in the authenticated organization.</param>
    /// <param name="reason">Optional explanation for the access change.</param>
    /// <param name="cancellationToken"></param>
    Task Pause(IEnumerable<string> userIds, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resume access across the organization for frontline members.
    /// Success means the request was queued, not that all updates have completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Targets must be frontline members in the authenticated organization. Valid users are queued even if other targets are invalid.
    /// If some users are rejected, a <see cref="SlackException"/> is thrown with a <c>failed_for_some_users</c> error.
    /// When none are valid, it is thrown with a <c>failed_for_all_users</c> error, and nothing is queued.
    /// Read the rejected users with <see cref="SlackException.ErrorField{T}"/> using <c>invalid_users</c>, as a list of <see cref="UserFailure"/>.
    /// For a partially accepted request, correct and retry only the rejected IDs.
    /// Authentication and queueing failures return an error without queueing any users.
    /// </para>
    /// <para>
    /// A successful response means the request was queued.
    /// Updates happen asynchronously, and failures during job execution are not returned in this response.
    /// Resuming an already-resumed member succeeds without changing their state.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.access.resume/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userIds">IDs of frontline members in the authenticated organization.</param>
    /// <param name="reason">Optional explanation for the access change.</param>
    /// <param name="cancellationToken"></param>
    Task Resume(IEnumerable<string> userIds, string? reason = null, CancellationToken cancellationToken = default);
}

public class AdminUsersAccessApi(ISlackApiClient client) : IAdminUsersAccessApi
{
    public Task Pause(IEnumerable<string> userIds, string? reason = null, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.access.pause", new Args
            {
                { "user_ids", userIds },
                { "reason", reason }
            }, cancellationToken);

    public Task Resume(IEnumerable<string> userIds, string? reason = null, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.access.resume", new Args
            {
                { "user_ids", userIds },
                { "reason", reason }
            }, cancellationToken);
}
