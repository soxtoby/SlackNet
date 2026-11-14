#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages users, their roles and guest expirations, and their workspace memberships, in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminUsersApi
{
    /// <summary>
    /// Add an Enterprise user to a workspace.
    /// </summary>
    /// <remarks>
    /// This Admin API assigns a user to a workspace. If the user has never been a member of the workspace, they will be added.
    /// If they've previously been removed or left the workspace, the user will be reinstated as a member.
    /// If this method is used on a workspace-level user who is deactivated on the org level, the user will be reactivated on both the workspace and the org level.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.assign/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamId">The ID (<c>T1234</c>) of the workspace.</param>
    /// <param name="userId">The ID of the user to add to the workspace.</param>
    /// <param name="isRestricted">True if user should be added to the workspace as a guest.</param>
    /// <param name="isUltraRestricted">True if user should be added to the workspace as a single-channel guest.</param>
    /// <param name="channelIds">Channel IDs to add user in the new workspace.</param>
    /// <param name="cancellationToken"></param>
    Task Assign(
        string teamId,
        string userId,
        bool isRestricted = false,
        bool isUltraRestricted = false,
        IEnumerable<string>? channelIds = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivate a user from an enterprise organization and all its workspaces.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.users.deactivate/">Slack documentation</a> for more information.</remarks>
    /// <param name="userId">The ID of the user to deactivate.</param>
    /// <param name="cancellationToken"></param>
    Task Deactivate(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the expiration timestamp for a guest.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If you do not provide a <paramref name="targetTeamId"/>, the guest's expiration timestamp on the current team or org is returned.
    /// If you do provide a <paramref name="targetTeamId"/>, you must also use an org token. The guest's expiration on the specified target team is returned.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.getExpiration/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userId">The ID of the guest user to get the expiration for.</param>
    /// <param name="targetTeamId">
    /// If an org token is passed in and this team is on the org, it will operate on the workspace level on the specified team.
    /// Otherwise it will operate on the org or team in context.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>The guest, with its <see cref="AdminUser.ExpirationTs"/>.</returns>
    Task<AdminUser> GetExpiration(string userId, string? targetTeamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invite a user to a workspace.
    /// </summary>
    /// <remarks>
    /// This Admin API invites a user to a workspace.
    /// If this method is used on a workspace-level user who is deactivated on the org level, the user will be reactivated on both the workspace and the org level.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.invite/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamId">The ID (<c>T1234</c>) of the workspace.</param>
    /// <param name="email">The email address of the person to invite.</param>
    /// <param name="channelIds">The IDs of channels for this user to join. At least one channel is required.</param>
    /// <param name="customMessage">An optional message to send to the user in the invite email.</param>
    /// <param name="realName">Full name of the user.</param>
    /// <param name="resend">
    /// Allow this invite to be resent in the future if a user has not signed up yet.
    /// Resending can only be done via the UI and has no expiration.
    /// </param>
    /// <param name="isRestricted">Is this user a multi-channel guest user?</param>
    /// <param name="isUltraRestricted">Is this user a single channel guest user?</param>
    /// <param name="guestExpiration">
    /// When guest account should be disabled.
    /// Only include this if you are inviting a guest user and you want their account to expire on a certain date.
    /// </param>
    /// <param name="emailPasswordPolicyEnabled">
    /// Allow invited user to sign in via email and password.
    /// Only available for Enterprise org teams via admin invite.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task Invite(
        string teamId,
        string email,
        IEnumerable<string> channelIds,
        string? customMessage = null,
        string? realName = null,
        bool resend = false,
        bool isRestricted = false,
        bool isUltraRestricted = false,
        DateTime? guestExpiration = null,
        bool? emailPasswordPolicyEnabled = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List users on a workspace.
    /// </summary>
    /// <remarks>
    /// This Admin API lists users in a workspace.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.list/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamId">The ID (<c>T1234</c>) of a workspace. Filters results to just the specified workspace.</param>
    /// <param name="isActive">
    /// If true, only active users will be returned. If false, only deactivated users will be returned. Default is true.
    /// </param>
    /// <param name="includeDeactivatedUserWorkspaces">
    /// Only applies with org token and no <paramref name="teamId"/>.
    /// If true, return <see cref="AdminUser.Workspaces"/> for a user even if they may be deactivated on them.
    /// If false, return <see cref="AdminUser.Workspaces"/> for a user only when user is active on them.
    /// </param>
    /// <param name="onlyGuests">If true, returns only guests and their expiration dates that belong to the <paramref name="teamId"/>.</param>
    /// <param name="includeAdmins">
    /// If true, only admin users will be returned (excludes owners).
    /// Returns all admins and owners when combined with <paramref name="includeOwners"/>.
    /// Cannot be used together with <paramref name="onlyGuests"/>.
    /// </param>
    /// <param name="includeOwners">If true, only owner users will be returned. Cannot be used together with <paramref name="onlyGuests"/>.</param>
    /// <param name="limit">Limit for how many users to be retrieved per page.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="AdminUserListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<AdminUserListResponse> List(
        string? teamId = null,
        bool? isActive = null,
        bool includeDeactivatedUserWorkspaces = false,
        bool onlyGuests = false,
        bool includeAdmins = false,
        bool includeOwners = false,
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a user from a workspace.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.users.remove/">Slack documentation</a> for more information.</remarks>
    /// <param name="teamId">The ID (<c>T1234</c>) of the workspace.</param>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <param name="cancellationToken"></param>
    Task Remove(string teamId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set an existing regular user or owner to be a workspace or org admin.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use a user's workspace ID to convert the user at the workspace level, and use a user's org ID to convert the user at the org level.
    /// </para>
    /// <para>
    /// When you convert a user at the org level, the user also becomes a Workspace Owner on every workspace in the org they belong to.
    /// To remove those roles, use <see cref="SetRegular"/> with the org ID.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.setAdmin/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamId">The ID of the workspace or organization.</param>
    /// <param name="userId">The ID of the user to designate as an admin.</param>
    /// <param name="cancellationToken"></param>
    Task SetAdmin(string teamId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set an expiration for a guest user.
    /// </summary>
    /// <remarks>
    /// This Admin API method sets the expiration of a single-channel or multi-channel guest, processed at minutes 30 and 45 to account for all timezones.
    /// For example, if a guest is set to expire at 11:10 am PDT, they will expire at 11:30 am PDT.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.setExpiration/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="userId">The ID of the user to set an expiration for.</param>
    /// <param name="expiration">When guest account should be disabled.</param>
    /// <param name="teamId">The ID (<c>T1234</c>) of the workspace.</param>
    /// <param name="cancellationToken"></param>
    Task SetExpiration(string userId, DateTime expiration, string? teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set an existing regular user or admin to be a workspace or org owner.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use a user's workspace ID to convert the user at the workspace level, and use a user's org ID to convert the user at the org level.
    /// </para>
    /// <para>
    /// When you convert a user at the org level, the user also becomes a Workspace Owner on every workspace in the org they belong to.
    /// To remove those roles, use <see cref="SetRegular"/> with the org ID.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.setOwner/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamId">The ID of the workspace or organization.</param>
    /// <param name="userId">ID of the user to promote to owner.</param>
    /// <param name="cancellationToken"></param>
    Task SetOwner(string teamId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set an existing guest user, admin user, or owner to be a regular user.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use a user's workspace ID to convert the user at the workspace level, and use a user's org ID to convert the user at the org level.
    /// </para>
    /// <para>
    /// When you convert a user at the org level, the user is also set to a regular user on every workspace in the org where they're a Workspace Owner.
    /// This matches demoting an org admin or owner from the admin dashboard.
    /// The change applies to every Workspace Owner role the user holds, including roles they had before becoming an org admin or owner,
    /// so call <see cref="SetOwner"/> with a workspace ID afterward to restore any you want to keep.
    /// </para>
    /// <para>
    /// If the org-level change succeeds but the workspace-level changes fail,
    /// a <see cref="SlackException"/> is thrown with a <c>failed_to_set_regular_on_workspaces</c> error. Call the method again to retry.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.setRegular/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamId">The ID of the workspace or organization.</param>
    /// <param name="userId">The ID of the user to designate as a regular user.</param>
    /// <param name="cancellationToken"></param>
    Task SetRegular(string teamId, string userId, CancellationToken cancellationToken = default);
}

public class AdminUsersApi(ISlackApiClient client) : IAdminUsersApi
{
    public Task Assign(
        string teamId,
        string userId,
        bool isRestricted = false,
        bool isUltraRestricted = false,
        IEnumerable<string>? channelIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("admin.users.assign", new Args
            {
                { "team_id", teamId },
                { "user_id", userId },
                { "is_restricted", isRestricted },
                { "is_ultra_restricted", isUltraRestricted },
                { "channel_ids", channelIds is null ? null : string.Join(",", channelIds) }
            }, cancellationToken);

    public Task Deactivate(string userId, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.deactivate", new Args { { "user_id", userId } }, cancellationToken);

    public async Task<AdminUser> GetExpiration(string userId, string? targetTeamId = null, CancellationToken cancellationToken = default) =>
        (await client.Get<AdminUserResponse>("admin.users.getExpiration", new Args
            {
                { "user_id", userId },
                { "target_team", targetTeamId }
            }, cancellationToken).ConfigureAwait(false))
        .User;

    public Task Invite(
        string teamId,
        string email,
        IEnumerable<string> channelIds,
        string? customMessage = null,
        string? realName = null,
        bool resend = false,
        bool isRestricted = false,
        bool isUltraRestricted = false,
        DateTime? guestExpiration = null,
        bool? emailPasswordPolicyEnabled = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("admin.users.invite", new Args
            {
                { "team_id", teamId },
                { "email", email },
                { "channel_ids", string.Join(",", channelIds) },
                { "custom_message", customMessage },
                { "real_name", realName },
                { "resend", resend },
                { "is_restricted", isRestricted },
                { "is_ultra_restricted", isUltraRestricted },
                { "guest_expiration_ts", guestExpiration?.ToTimestamp() },
                { "email_password_policy_enabled", emailPasswordPolicyEnabled }
            }, cancellationToken);

    public Task<AdminUserListResponse> List(
        string? teamId = null,
        bool? isActive = null,
        bool includeDeactivatedUserWorkspaces = false,
        bool onlyGuests = false,
        bool includeAdmins = false,
        bool includeOwners = false,
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<AdminUserListResponse>("admin.users.list", new Args
            {
                { "team_id", teamId },
                { "is_active", isActive },
                { "include_deactivated_user_workspaces", includeDeactivatedUserWorkspaces },
                { "only_guests", onlyGuests },
                { "include_admins", includeAdmins },
                { "include_owners", includeOwners },
                { "limit", limit },
                { "cursor", cursor }
            }, cancellationToken);

    public Task Remove(string teamId, string userId, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.remove", new Args
            {
                { "team_id", teamId },
                { "user_id", userId }
            }, cancellationToken);

    public Task SetAdmin(string teamId, string userId, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.setAdmin", new Args
            {
                { "team_id", teamId },
                { "user_id", userId }
            }, cancellationToken);

    public Task SetExpiration(string userId, DateTime expiration, string? teamId = null, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.setExpiration", new Args
            {
                { "user_id", userId },
                { "expiration_ts", expiration.ToTimestampNumber() },
                { "team_id", teamId }
            }, cancellationToken);

    public Task SetOwner(string teamId, string userId, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.setOwner", new Args
            {
                { "team_id", teamId },
                { "user_id", userId }
            }, cancellationToken);

    public Task SetRegular(string teamId, string userId, CancellationToken cancellationToken = default) =>
        client.Post("admin.users.setRegular", new Args
            {
                { "team_id", teamId },
                { "user_id", userId }
            }, cancellationToken);
}
