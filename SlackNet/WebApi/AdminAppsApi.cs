#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Approves, restricts and uninstalls apps in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
/// <remarks>See <a href="https://docs.slack.dev/admins/managing-app-approvals/">managing app approvals</a> for more information.</remarks>
public interface IAdminAppsApi
{
    /// <summary>
    /// Approve an app install request, approve an app for a particular workspace, or approve an app across an entire Enterprise organization.
    /// When approved for an Enterprise organization, an app can still be independently restricted (see <see cref="Restrict"/>) on particular workspaces.
    /// </summary>
    /// <remarks>
    /// <para>One of <paramref name="teamId"/> or <paramref name="enterpriseId"/> is required, not both.</para>
    /// <para>
    /// Either <paramref name="appId"/> or <paramref name="requestId"/> is required.
    /// These IDs can be obtained either directly via the <see cref="Events.AppRequested"/> event, or by <see cref="IAdminAppsRequestsApi.List"/>.
    /// When an <paramref name="appId"/> is provided, the request will approve what is currently listed in the app's manifest
    /// (regardless of whether it has changed since the time of request).
    /// When a <paramref name="requestId"/> is provided, the request will be approved and what was asked for in the request is granted.
    /// </para>
    /// <para>
    /// If an app was previously approved/resolved at an org level, it will need to be re-approved at the org level upon any scope changes.
    /// Reinstalling the app to the workspace is required to add new scopes, but no approval at the workspace level is required.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.apps.approve/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="appId">The ID of the app to approve.</param>
    /// <param name="requestId">The ID of the request to approve.</param>
    /// <param name="teamId">The ID of the workspace to approve the app on.</param>
    /// <param name="enterpriseId">The ID of the enterprise to approve the app on.</param>
    /// <param name="userScopes">
    /// User scopes to approve for the app.
    /// If neither this nor <paramref name="botScopes"/> are provided, all requested scopes will be granted, including optional ones.
    /// To limit the scopes granted, provide all required scopes along with the subset of optional scopes (see <see cref="AppScope.IsOptional"/>) you want to approve.
    /// </param>
    /// <param name="botScopes">
    /// Bot scopes to approve for the app.
    /// If neither this nor <paramref name="userScopes"/> are provided, all requested scopes will be granted, including optional ones.
    /// To limit the scopes granted, provide all required scopes along with the subset of optional scopes (see <see cref="AppScope.IsOptional"/>) you want to approve.
    /// </param>
    /// <param name="allowChildAutoInstall">Auto-create an Admin-Approved App automation rule that pre-approves future child app installs from this manager app.</param>
    /// <param name="cancellationToken"></param>
    Task Approve(
        string? appId = null,
        string? requestId = null,
        string? teamId = null,
        string? enterpriseId = null,
        IEnumerable<string>? userScopes = null,
        IEnumerable<string>? botScopes = null,
        bool allowChildAutoInstall = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears an app resolution set for an app on a specific workspace or enterprise,
    /// undoing the effect of <see cref="Approve"/> or <see cref="Restrict"/>.
    /// </summary>
    /// <remarks>
    /// Exactly one of <paramref name="teamId"/> or <paramref name="enterpriseId"/> is required, not both.
    /// Providing an <paramref name="enterpriseId"/> clears the resolution at the enterprise level, not on each of the enterprise's workspaces.
    /// The <paramref name="teamId"/> clears the resolution on a single workspace.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.apps.clearResolution/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="appId">The ID of the app whose resolution you want to clear/undo.</param>
    /// <param name="teamId">The workspace to clear the app resolution from.</param>
    /// <param name="enterpriseId">The enterprise to clear the app resolution from.</param>
    /// <param name="cancellationToken"></param>
    Task ClearResolution(string appId, string? teamId = null, string? enterpriseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Denies an app install request, restricts an app for a specific workspace, or restricts an app across an entire enterprise.
    /// When restricted for the enterprise, an app can not be independently approved (see <see cref="Approve"/>) on particular workspaces,
    /// and any existing workspace-level approvals are voided.
    /// Restricting an app does not remove existing installations of the app, it only prevents future installation.
    /// </summary>
    /// <remarks>
    /// <para>Exactly one of <paramref name="teamId"/> or <paramref name="enterpriseId"/> is required, not both.</para>
    /// <para>
    /// Either <paramref name="appId"/> or <paramref name="requestId"/> is required.
    /// These IDs can be obtained either directly via the <see cref="Events.AppRequested"/> event, or by <see cref="IAdminAppsRequestsApi.List"/>.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.apps.restrict/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="appId">The ID of the app to restrict.</param>
    /// <param name="requestId">The ID of the request to restrict.</param>
    /// <param name="teamId">The ID of the workspace to restrict the app on.</param>
    /// <param name="enterpriseId">The ID of the enterprise to restrict the app on.</param>
    /// <param name="cancellationToken"></param>
    Task Restrict(
        string? appId = null,
        string? requestId = null,
        string? teamId = null,
        string? enterpriseId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Uninstall an app from one or many workspaces, or an entire enterprise organization.
    /// Uninstalling an app cannot be undone.
    /// Once an application is uninstalled, users must reinstall it manually if they wish to restore it.
    /// </summary>
    /// <remarks>
    /// <para>Exactly one of <paramref name="teamIds"/> or <paramref name="enterpriseId"/> is required to specify where the app should be removed.</para>
    /// <para>
    /// Admin apps that work with SCIM, app management, or certain other enterprise-level features cannot be uninstalled using this API.
    /// To remove one of those, please contact Slack via the <a href="https://slack.com/help">Slack Help Center</a>.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.apps.uninstall/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="appId">The ID of the app to uninstall.</param>
    /// <param name="teamIds">IDs of the teams to uninstall from.</param>
    /// <param name="enterpriseId">The enterprise to completely uninstall the application from (across all workspaces).</param>
    /// <param name="cancellationToken"></param>
    Task Uninstall(string appId, IEnumerable<string>? teamIds = null, string? enterpriseId = null, CancellationToken cancellationToken = default);
}

public class AdminAppsApi(ISlackApiClient client) : IAdminAppsApi
{
    public Task Approve(
        string? appId = null,
        string? requestId = null,
        string? teamId = null,
        string? enterpriseId = null,
        IEnumerable<string>? userScopes = null,
        IEnumerable<string>? botScopes = null,
        bool allowChildAutoInstall = false,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("admin.apps.approve", new Args
            {
                { "app_id", appId },
                { "request_id", requestId },
                { "team_id", teamId },
                { "enterprise_id", enterpriseId },
                { "user_scopes", userScopes is null ? null : string.Join(",", userScopes) },
                { "bot_scopes", botScopes is null ? null : string.Join(",", botScopes) },
                { "allow_child_auto_install", allowChildAutoInstall }
            }, cancellationToken);

    public Task ClearResolution(string appId, string? teamId = null, string? enterpriseId = null, CancellationToken cancellationToken = default) =>
        client.Post("admin.apps.clearResolution", new Args
            {
                { "app_id", appId },
                { "team_id", teamId },
                { "enterprise_id", enterpriseId }
            }, cancellationToken);

    public Task Restrict(
        string? appId = null,
        string? requestId = null,
        string? teamId = null,
        string? enterpriseId = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("admin.apps.restrict", new Args
            {
                { "app_id", appId },
                { "request_id", requestId },
                { "team_id", teamId },
                { "enterprise_id", enterpriseId }
            }, cancellationToken);

    public Task Uninstall(string appId, IEnumerable<string>? teamIds = null, string? enterpriseId = null, CancellationToken cancellationToken = default) =>
        client.Post("admin.apps.uninstall", new Args
            {
                { "app_id", appId },
                { "team_ids", teamIds is null ? null : string.Join(",", teamIds) },
                { "enterprise_id", enterpriseId }
            }, cancellationToken);
}
