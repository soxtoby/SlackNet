#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages which entities are assigned to authentication policies in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminAuthPolicyApi
{
    /// <summary>
    /// Assigns entities (currently, users) to an authentication policy—for example, signing in with email and password.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.auth.policy.assignEntities/">Slack documentation</a> for more information.</remarks>
    /// <param name="policyName">
    /// The name of the authentication policy to assign the entities to.
    /// Currently, <c>email_password</c> is the only policy that may be used with this method.
    /// </param>
    /// <param name="entityType">The type of entity to assign to the policy.</param>
    /// <param name="entityIds">IDs to assign to the policy.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Count of entities under the policy.</returns>
    Task<int> AssignEntities(
        string policyName,
        AuthPolicyEntityType entityType,
        IEnumerable<string> entityIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetch all the entities (currently, users) assigned to a particular authentication policy by name—for example, signing in with email and password.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.auth.policy.getEntities/">Slack documentation</a> for more information.</remarks>
    /// <param name="policyName">
    /// The name of the policy to fetch entities for.
    /// Currently, <c>email_password</c> is the only policy that may be used with this method.
    /// </param>
    /// <param name="entityType">The type of entity to fetch.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="AuthPolicyEntityListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The maximum number of items to return. Defaults to 1000.</param>
    /// <param name="cancellationToken"></param>
    Task<AuthPolicyEntityListResponse> GetEntities(
        string policyName,
        AuthPolicyEntityType? entityType = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes entities (currently, users) from an authentication policy—for example, signing in with email and password.
    /// After being removed, the entities will authenticate using the default policy on your organization, which might be a policy like Single-Sign On.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.auth.policy.removeEntities/">Slack documentation</a> for more information.</remarks>
    /// <param name="policyName">
    /// The name of the policy to remove entities from.
    /// Currently, <c>email_password</c> is the only policy that may be used with this method.
    /// </param>
    /// <param name="entityType">The type of entity to remove from the policy.</param>
    /// <param name="entityIds">Encoded IDs of the entities you'd like to remove from the policy.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Count of entities under the policy.</returns>
    Task<int> RemoveEntities(
        string policyName,
        AuthPolicyEntityType entityType,
        IEnumerable<string> entityIds,
        CancellationToken cancellationToken = default);
}

public class AdminAuthPolicyApi(ISlackApiClient client) : IAdminAuthPolicyApi
{
    public async Task<int> AssignEntities(
        string policyName,
        AuthPolicyEntityType entityType,
        IEnumerable<string> entityIds,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<AuthPolicyEntityCountResponse>("admin.auth.policy.assignEntities", new Args
            {
                { "policy_name", policyName },
                { "entity_type", entityType },
                { "entity_ids", entityIds }
            }, cancellationToken).ConfigureAwait(false)).EntityTotalCount;

    public Task<AuthPolicyEntityListResponse> GetEntities(
        string policyName,
        AuthPolicyEntityType? entityType = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<AuthPolicyEntityListResponse>("admin.auth.policy.getEntities", new Args
            {
                { "policy_name", policyName },
                { "entity_type", entityType },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);

    public async Task<int> RemoveEntities(
        string policyName,
        AuthPolicyEntityType entityType,
        IEnumerable<string> entityIds,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<AuthPolicyEntityCountResponse>("admin.auth.policy.removeEntities", new Args
            {
                { "policy_name", policyName },
                { "entity_type", entityType },
                { "entity_ids", entityIds }
            }, cancellationToken).ConfigureAwait(false)).EntityTotalCount;
}
