#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IFunctionDistributionPermissionsApi
{
    /// <summary>
    /// Grant users access to a custom slack function if its permission_type is set to named_entities.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/functions.distributions.permissions.add">Slack documentation</a> for more information.</remarks>
    /// <param name="functionId">The ID of the function.</param>
    /// <param name="functionCallbackId">The callback ID defined in the function's definition file.</param>
    /// <param name="functionAppId">The ID of the app.</param>
    /// <param name="userIds">List of user IDs.</param>
    /// <param name="cancellationToken"></param>
    Task<FunctionPermissionsResponse> Add(
        string? functionId = null,
        string? functionCallbackId = null,
        string? functionAppId = null,
        IEnumerable<string>? userIds = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// List the access type of a custom slack function and include the users,
    /// team, or org ids with access if its permission_type is set to named_entities.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/functions.distributions.permissions.list">Slack documentation</a> for more information.</remarks>
    /// <param name="functionId">The ID of the function,</param>
    /// <param name="functionCallbackId">The callback ID defined in the function's definition file.</param>
    /// <param name="functionAppId">The ID of the app.</param>
    /// <param name="cancellationToken"></param>
    Task<FunctionPermissionsResponse> List(string? functionId = null, string? functionCallbackId = null, string? functionAppId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke user access to a custom slack function if permission_type set to named_entities.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/functions.distributions.permissions.remove">Slack documentation</a> for more information.</remarks>
    /// <param name="functionId">The ID of the function.</param>
    /// <param name="functionCallbackId">The callback ID defined in the function's definition file.</param>
    /// <param name="functionAppId">The ID of the app.</param>
    /// <param name="userIds">List of user IDs.</param>
    /// <param name="cancellationToken"></param>
    Task<FunctionPermissionsResponse> Remove(string? functionId = null, string? functionCallbackId = null, string? functionAppId = null, IEnumerable<string>? userIds = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Set the access type of a custom slack function and define the users,
    /// team or org ids to be granted access if permission_type is set to named_entities.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/functions.distributions.permissions.set">Slack documentation</a> for more information.</remarks>
    /// <param name="functionId">The ID of the function.</param>
    /// <param name="functionCallbackId">The callback ID defined in the function's definition file.</param>
    /// <param name="functionAppId">The ID of the app.</param>
    /// <param name="permissionType">The type of permission that defines how the function can be distributed.</param>
    /// <param name="userIds">List of user IDs.</param>
    /// <param name="teamIds">List of team IDs to allow for named_entities permission.</param>
    /// <param name="orgIds">List of org IDs to allow for named_entities permission.</param>
    /// <param name="cancellationToken"></param>
    Task<FunctionPermissionsResponse> Set(
        string? functionId = null,
        string? functionCallbackId = null,
        string? functionAppId = null,
        FunctionPermissionType? permissionType = null,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? teamIds = null,
        IEnumerable<string>? orgIds = null,
        CancellationToken cancellationToken = default);
}

public class FunctionDistributionPermissionsApi(ISlackApiClient client) : IFunctionDistributionPermissionsApi
{
    public Task<FunctionPermissionsResponse> Add(
        string? functionId = null,
        string? functionCallbackId = null,
        string? functionAppId = null,
        IEnumerable<string>? userIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<FunctionPermissionsResponse>("functions.distributions.permissions.add", new Args
            {
                { "function_id", functionId },
                { "function_callback_id", functionCallbackId },
                { "function_app_id", functionAppId },
                { "user_ids", userIds }
            }, cancellationToken);
    
    public Task<FunctionPermissionsResponse> List(
        string? functionId = null,
        string? functionCallbackId = null,
        string? functionAppId = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<FunctionPermissionsResponse>("functions.distributions.permissions.list", new Args
            {
                { "function_id", functionId },
                { "function_callback_id", functionCallbackId },
                { "function_app_id", functionAppId }
            }, cancellationToken);

    public Task<FunctionPermissionsResponse> Remove(
        string? functionId = null,
        string? functionCallbackId = null,
        string? functionAppId = null,
        IEnumerable<string>? userIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<FunctionPermissionsResponse>("functions.distributions.permissions.remove", new Args
            {
                { "function_id", functionId },
                { "function_callback_id", functionCallbackId },
                { "function_app_id", functionAppId },
                { "user_ids", userIds }
            }, cancellationToken);

    public Task<FunctionPermissionsResponse> Set(
        string? functionId = null,
        string? functionCallbackId = null,
        string? functionAppId = null,
        FunctionPermissionType? permissionType = null,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? teamIds = null,
        IEnumerable<string>? orgIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<FunctionPermissionsResponse>("functions.distributions.permissions.set", new Args
            {
                { "function_id", functionId },
                { "function_callback_id", functionCallbackId },
                { "function_app_id", functionAppId },
                { "permission_type", permissionType },
                { "user_ids", userIds },
                { "team_ids", teamIds },
                { "org_ids", orgIds }
            }, cancellationToken);
}
