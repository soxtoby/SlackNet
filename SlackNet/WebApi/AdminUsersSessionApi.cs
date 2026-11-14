#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Lists, invalidates and resets users' login sessions, and manages their session settings, in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminUsersSessionApi
{
    /// <summary>
    /// Clear user-specific session settings—the session duration and what happens when the client closes—for a list of users.
    /// </summary>
    /// <remarks>
    /// If you'd like to set session settings for a given user, use <see cref="SetSettings"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.session.clearSettings/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userIds">The IDs of users you'd like to clear session settings for.</param>
    /// <param name="cancellationToken"></param>
    Task ClearSettings(IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user-specific session settings—the session duration and what happens when the client closes—given a list of users.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Session settings include a duration (the amount of time a session can last) and whether the session should be terminated when the client is closed.
    /// If a provided user does not have any session settings, they will be omitted from <see cref="UserSessionSettingsResponse.SessionSettings"/>.
    /// </para>
    /// <para>If you'd like to set session settings for a given user, use <see cref="SetSettings"/>.</para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.session.getSettings/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userIds">
    /// The IDs of users you'd like to fetch session settings for.
    /// Note: if a user does not have any active sessions, they will not be returned in the response.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<UserSessionSettingsResponse> GetSettings(IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke a single session for a user. The user will be forced to login to Slack.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This Admin API invalidates a <b>single</b> session for a user on a workspace. The user will be logged out and forced to login again.
    /// If the user has multiple sessions with multiple devices, the other sessions will be unaffected.
    /// </para>
    /// <para>If you'd like to reset <i>all sessions</i> for a given user, use <see cref="Reset"/> instead.</para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.session.invalidate/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userId">ID of the user that the session belongs to.</param>
    /// <param name="sessionId">ID of the session to invalidate.</param>
    /// <param name="cancellationToken"></param>
    Task Invalidate(string userId, long sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// List active user sessions for an organization.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If no <paramref name="userId"/> and <paramref name="teamId"/> are passed, you'll receive a paginated list of <i>all</i> sessions.
    /// When you pass <paramref name="userId"/> and <paramref name="teamId"/> (which must be used together),
    /// you'll receive a list of active sessions by that user on the workspace specified by <paramref name="teamId"/>.
    /// </para>
    /// <para>
    /// These sessions can be used to reset a session with <see cref="Invalidate"/>: the user will be logged out and forced to login again.
    /// If the user has multiple sessions with multiple devices, the other sessions will be unaffected.
    /// If you'd like to reset <i>all sessions</i> for a given user, use <see cref="Reset"/> instead.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.session.list/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userId">The ID of user you'd like active sessions for. If you pass a <paramref name="userId"/>, you'll need to pass a <paramref name="teamId"/> as well.</param>
    /// <param name="teamId">The ID of the workspace you'd like active sessions for. If you pass a <paramref name="teamId"/>, you'll need to pass a <paramref name="userId"/> as well.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="ActiveSessionListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<ActiveSessionListResponse> List(
        string? userId = null,
        string? teamId = null,
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Wipes all valid sessions on all devices for a given user.
    /// </summary>
    /// <remarks>
    /// <para>This method wipes a user session, leaving the user unauthenticated. The user's Slack client will reset its local cache.</para>
    /// <para>
    /// By default, this method resets all sessions for a given user.
    /// Use <paramref name="webOnly"/> and <paramref name="mobileOnly"/> to wipe only web or only mobile sessions.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.session.reset/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userId">The ID of the user to wipe sessions for.</param>
    /// <param name="mobileOnly">Only expire mobile sessions.</param>
    /// <param name="webOnly">Only expire web sessions.</param>
    /// <param name="cancellationToken"></param>
    Task Reset(string userId, bool mobileOnly = false, bool webOnly = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enqueues an asynchronous job to wipe all valid sessions on all devices for a given list of users.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method enqueues a job that wipes sessions for a list of users, leaving them unauthenticated.
    /// The users' Slack clients will reset their local cache.
    /// </para>
    /// <para>
    /// By default, the enqueued job will reset all sessions for the given users.
    /// Use <paramref name="webOnly"/> and <paramref name="mobileOnly"/> to wipe only web or only mobile sessions.
    /// </para>
    /// <para>
    /// A successful response does not mean all sessions have been reset. This process is asynchronous.
    /// It means that the job that performs the reset has successfully been enqueued.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.session.resetBulk/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userIds">The list of user IDs to wipe sessions for.</param>
    /// <param name="mobileOnly">Only expire mobile sessions.</param>
    /// <param name="webOnly">Only expire web sessions.</param>
    /// <param name="cancellationToken"></param>
    Task ResetBulk(IEnumerable<string> userIds, bool mobileOnly = false, bool webOnly = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Configure the user-level session settings—the session duration and what happens when the client closes—for one or more users.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.users.session.setSettings/">Slack documentation</a> for more information.</remarks>
    /// <param name="userIds">The list of user IDs to apply the session settings for.</param>
    /// <param name="duration">The session duration, in whole seconds.</param>
    /// <param name="desktopAppBrowserQuit">Terminate the session when the client—either the desktop app or a browser window—is closed.</param>
    /// <param name="cancellationToken"></param>
    Task SetSettings(
        IEnumerable<string> userIds,
        TimeSpan? duration = null,
        bool? desktopAppBrowserQuit = null,
        CancellationToken cancellationToken = default);
}

public class AdminUsersSessionApi(ISlackApiClient client) : IAdminUsersSessionApi
{
    public Task ClearSettings(IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.session.clearSettings", new Args { { "user_ids", userIds } }, cancellationToken);

    public Task<UserSessionSettingsResponse> GetSettings(IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
        client.Post<UserSessionSettingsResponse>("admin.users.session.getSettings", new Args { { "user_ids", userIds } }, cancellationToken);

    public Task Invalidate(string userId, long sessionId, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.session.invalidate", new Args
            {
                { "user_id", userId },
                { "session_id", sessionId }
            }, cancellationToken);

    public Task<ActiveSessionListResponse> List(
        string? userId = null,
        string? teamId = null,
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<ActiveSessionListResponse>("admin.users.session.list", new Args
            {
                { "user_id", userId },
                { "team_id", teamId },
                { "limit", limit },
                { "cursor", cursor }
            }, cancellationToken);

    public Task Reset(string userId, bool mobileOnly = false, bool webOnly = false, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.session.reset", new Args
            {
                { "user_id", userId },
                { "mobile_only", mobileOnly },
                { "web_only", webOnly }
            }, cancellationToken);

    public Task ResetBulk(IEnumerable<string> userIds, bool mobileOnly = false, bool webOnly = false, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.session.resetBulk", new Args
            {
                { "user_ids", userIds },
                { "mobile_only", mobileOnly },
                { "web_only", webOnly }
            }, cancellationToken);

    public Task SetSettings(
        IEnumerable<string> userIds,
        TimeSpan? duration = null,
        bool? desktopAppBrowserQuit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("admin.users.session.setSettings", new Args
            {
                { "user_ids", userIds },
                { "duration", (int?)duration?.TotalSeconds },
                { "desktop_app_browser_quit", desktopAppBrowserQuit }
            }, cancellationToken);
}
