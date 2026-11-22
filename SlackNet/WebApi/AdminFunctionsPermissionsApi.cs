using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages the visibility of functions in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminFunctionsPermissionsApi
{
    /// <summary>
    /// Lookup the visibility of multiple Slack functions and include the users if it is limited to particular named entities.
    /// </summary>
    /// <remarks>
    /// Look up permissions for a set of functions.
    /// This method returns a map of functions to permissions objects and a map of any errors found during permission lookup.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.functions.permissions.lookup/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="functionIds">An array of function IDs to get permissions for.</param>
    /// <param name="cancellationToken"></param>
    Task<FunctionPermissionsLookupResponse> Lookup(IEnumerable<string> functionIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the visibility of a Slack function and define the users or workspaces if it is set to <see cref="FunctionPermissionType.NamedEntities"/>.
    /// </summary>
    /// <remarks>
    /// Set permissions for a function. This method will overwrite the permissions currently in place.
    /// An admin can only further restrict the permissions set by the function, not expand them.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.functions.permissions.set/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="functionId">The function ID to set permissions for.</param>
    /// <param name="visibility">The function visibility.</param>
    /// <param name="userIds">List of user IDs to allow for <see cref="FunctionPermissionType.NamedEntities"/> visibility.</param>
    /// <param name="permissions">Array of permissions for the function.</param>
    /// <param name="cancellationToken"></param>
    Task Set(
        string functionId,
        FunctionPermissionType? visibility = null,
        IEnumerable<string>? userIds = null,
        IEnumerable<FunctionPermissionSetting>? permissions = null,
        CancellationToken cancellationToken = default);
}

public class AdminFunctionsPermissionsApi(ISlackApiClient client) : IAdminFunctionsPermissionsApi
{
    public Task<FunctionPermissionsLookupResponse> Lookup(IEnumerable<string> functionIds, CancellationToken cancellationToken = default) =>
        client.Post<FunctionPermissionsLookupResponse>("admin.functions.permissions.lookup", new Args
            {
                { "function_ids", functionIds }
            }, cancellationToken);

    public Task Set(
        string functionId,
        FunctionPermissionType? visibility = null,
        IEnumerable<string>? userIds = null,
        IEnumerable<FunctionPermissionSetting>? permissions = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("admin.functions.permissions.set", new Args
            {
                { "function_id", functionId },
                { "visibility", visibility },
                { "user_ids", userIds },
                { "permissions", permissions }
            }, cancellationToken);
}
