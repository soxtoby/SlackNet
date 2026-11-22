using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages the users assigned to admin roles in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminRolesApi
{
    /// <summary>
    /// Adds members to the specified role with the specified scopes.
    /// </summary>
    /// <remarks>
    /// <para>Adds members to a specified role, granting them the permission scopes that are assigned to that role.</para>
    /// <para>
    /// The API will process all user, team, and org IDs passed.
    /// If any of the IDs are rejected, a <see cref="SlackException"/> is thrown with a <c>failed_for_some_entities</c>, <c>failed_for_some_users</c>
    /// or <c>failed_for_some_users_and_entities</c> error.
    /// Read the rejected IDs and the reasons they were rejected with <see cref="SlackException.ErrorField{T}"/>
    /// using <c>rejected_entities</c> and <c>rejected_users</c>, as lists of <see cref="RoleAssignmentRejection"/>.
    /// </para>
    /// <para>The following roles and their respective scope types can be assigned by Org Owners and Org Admins on Enterprise plans:</para>
    /// <list type="table">
    ///   <listheader><term>Role</term><description>Role ID (scope type)</description></listheader>
    ///   <item><term>Analytics Admin</term><description><c>Rl0L</c> (team or org)</description></item>
    ///   <item><term>Audit Logs Admin</term><description><c>Rl0C</c> (team or org)</description></item>
    ///   <item><term>Channel Admin</term><description><c>Rl01</c> (team or org)</description></item>
    ///   <item><term>Channel Manager</term><description><c>Rl0A</c> (team or org)</description></item>
    ///   <item><term>Conversation Admin</term><description><c>Rl05</c> (team or org)</description></item>
    ///   <item><term>DLP Admin</term><description><c>Rl09</c> (org)</description></item>
    ///   <item><term>Exports Admin</term><description><c>Rl0F</c> (org)</description></item>
    ///   <item><term>Integrations Manager</term><description><c>Rl0D</c> (org)</description></item>
    ///   <item><term>Legal Holds Admin</term><description><c>Rl04</c> (org)</description></item>
    ///   <item><term>Message Activity Manager</term><description><c>Rl0E</c> (org)</description></item>
    ///   <item><term>Role Admin</term><description><c>Rl02</c> (team or org)</description></item>
    ///   <item><term>Sales Admin</term><description><c>Rl0G</c> (org)</description></item>
    ///   <item><term>Security Admin</term><description><c>Rl0J</c> (org)</description></item>
    ///   <item><term>Slack Platform Developer</term><description><c>Rl0B</c> (team or org)</description></item>
    ///   <item><term>User Admin</term><description><c>Rl03</c> (team or org)</description></item>
    ///   <item><term>Workflow Admin</term><description><c>Rl0K</c> (org)</description></item>
    /// </list>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.roles.addAssignments/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="roleId">ID of the role to which users will be assigned.</param>
    /// <param name="entityIds">List of the entity IDs for which roles will be assigned. These can be Org IDs, Team IDs or Channel IDs.</param>
    /// <param name="userIds">List of IDs from the users to be added to the given role.</param>
    /// <param name="cancellationToken"></param>
    Task AddAssignments(
        string roleId,
        IEnumerable<string> entityIds,
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists assignments for all roles across entities.
    /// Options to scope results by any combination of roles or entities.
    /// </summary>
    /// <remarks>
    /// By default, this will return all existing role assignments across your entire org.
    /// Search for specific roles by providing the optional <paramref name="roleIds"/> argument.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.roles.listAssignments/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="roleIds">Collection of role IDs to scope results by.</param>
    /// <param name="entityIds">The entities for which the roles apply.</param>
    /// <param name="sortDirection">Sort direction. Default is descending on <see cref="RoleAssignment.DateCreate"/>.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="RoleAssignmentListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The maximum number of items to return. Defaults to 200.</param>
    /// <param name="cancellationToken"></param>
    Task<RoleAssignmentListResponse> ListAssignments(
        IEnumerable<string>? roleIds = null,
        IEnumerable<string>? entityIds = null,
        SortDirection? sortDirection = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a set of users from a role for the given scopes and entities.
    /// </summary>
    /// <remarks>
    /// <para>Revokes role assignments for specific users and entities across your entire organization.</para>
    /// <para>
    /// The API will remove role assignments for all users specified in <paramref name="userIds"/>
    /// for all org, team, or channel IDs specified in <paramref name="entityIds"/>.
    /// If any of the IDs are rejected, a <see cref="SlackException"/> is thrown with a <c>failed_for_some_entities</c>, <c>failed_for_some_users</c>
    /// or <c>failed_for_some_users_and_entities</c> error.
    /// Read the rejected IDs and the reasons they were rejected with <see cref="SlackException.ErrorField{T}"/>
    /// using <c>rejected_entities</c> and <c>rejected_users</c>, as lists of <see cref="RoleAssignmentRejection"/>.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.roles.removeAssignments/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="roleId">ID of the role from which users will be removed.</param>
    /// <param name="entityIds">List of the entity IDs for which roles will be revoked. These can be Org IDs, Team IDs or Channel IDs.</param>
    /// <param name="userIds">List of IDs of the users whose roles will be revoked.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveAssignments(
        string roleId,
        IEnumerable<string> entityIds,
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default);
}

public class AdminRolesApi(ISlackApiClient client) : IAdminRolesApi
{
    public Task AddAssignments(
        string roleId,
        IEnumerable<string> entityIds,
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("admin.roles.addAssignments", new Args
            {
                { "role_id", roleId },
                { "entity_ids", entityIds },
                { "user_ids", userIds }
            }, cancellationToken);

    public Task<RoleAssignmentListResponse> ListAssignments(
        IEnumerable<string>? roleIds = null,
        IEnumerable<string>? entityIds = null,
        SortDirection? sortDirection = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<RoleAssignmentListResponse>("admin.roles.listAssignments", new Args
            {
                { "role_ids", roleIds },
                { "entity_ids", entityIds },
                { "sort_dir", sortDirection },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);

    public Task RemoveAssignments(
        string roleId,
        IEnumerable<string> entityIds,
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("admin.roles.removeAssignments", new Args
            {
                { "role_id", roleId },
                { "entity_ids", entityIds },
                { "user_ids", userIds }
            }, cancellationToken);
}
