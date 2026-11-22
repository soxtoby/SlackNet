using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages information barriers in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminBarriersApi
{
    /// <summary>
    /// Creates a new information barrier.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.barriers.create/">Slack documentation</a> for more information.</remarks>
    /// <param name="primaryUsergroupId">
    /// The ID of the primary <a href="https://slack.com/help/articles/115001435788-Connect-identity-provider-groups-to-your-Enterprise-organization">IDP Group</a>.
    /// </param>
    /// <param name="barrieredFromUsergroupIds">
    /// A list of <a href="https://slack.com/help/articles/115001435788-Connect-identity-provider-groups-to-your-Enterprise-organization">IDP Group</a> IDs
    /// that the primary usergroup is to be barriered from.
    /// </param>
    /// <param name="restrictedSubjects">
    /// What kind of interactions are blocked by this barrier.
    /// Currently you must pass all three: <see cref="RestrictedSubject.Im"/>, <see cref="RestrictedSubject.Mpim"/> and <see cref="RestrictedSubject.Call"/>.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<InformationBarrier> Create(
        string primaryUsergroupId,
        IEnumerable<string> barrieredFromUsergroupIds,
        IEnumerable<RestrictedSubject> restrictedSubjects,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing information barrier.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.barriers.delete/">Slack documentation</a> for more information.</remarks>
    /// <param name="barrierId">The ID of the barrier you're trying to delete.</param>
    /// <param name="cancellationToken"></param>
    Task Delete(string barrierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all existing information barriers.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.barriers.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="filterByUsergroupName">Return only barriers whose usergroup name matches this value.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="InformationBarrierListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The maximum number of items to return. Defaults to 100.</param>
    /// <param name="cancellationToken"></param>
    Task<InformationBarrierListResponse> List(
        string? filterByUsergroupName = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing information barrier.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.barriers.update/">Slack documentation</a> for more information.</remarks>
    /// <param name="barrierId">The ID of the barrier you're trying to modify.</param>
    /// <param name="primaryUsergroupId">
    /// The ID of the primary <a href="https://slack.com/help/articles/115001435788-Connect-identity-provider-groups-to-your-Enterprise-organization">IDP Group</a>.
    /// </param>
    /// <param name="barrieredFromUsergroupIds">
    /// A list of <a href="https://slack.com/help/articles/115001435788-Connect-identity-provider-groups-to-your-Enterprise-organization">IDP Group</a> IDs
    /// that the primary usergroup is to be barriered from.
    /// </param>
    /// <param name="restrictedSubjects">
    /// What kind of interactions are blocked by this barrier.
    /// Currently you must pass all three: <see cref="RestrictedSubject.Im"/>, <see cref="RestrictedSubject.Mpim"/> and <see cref="RestrictedSubject.Call"/>.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<InformationBarrier> Update(
        string barrierId,
        string primaryUsergroupId,
        IEnumerable<string> barrieredFromUsergroupIds,
        IEnumerable<RestrictedSubject> restrictedSubjects,
        CancellationToken cancellationToken = default);
}

public class AdminBarriersApi(ISlackApiClient client) : IAdminBarriersApi
{
    public async Task<InformationBarrier> Create(
        string primaryUsergroupId,
        IEnumerable<string> barrieredFromUsergroupIds,
        IEnumerable<RestrictedSubject> restrictedSubjects,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Get<InformationBarrierResponse>("admin.barriers.create", new Args
            {
                { "primary_usergroup_id", primaryUsergroupId },
                { "barriered_from_usergroup_ids", barrieredFromUsergroupIds },
                { "restricted_subjects", restrictedSubjects }
            }, cancellationToken).ConfigureAwait(false)).Barrier;

    public Task Delete(string barrierId, CancellationToken cancellationToken = default) =>
        client.Get("admin.barriers.delete", new Args { { "barrier_id", barrierId } }, cancellationToken);

    public Task<InformationBarrierListResponse> List(
        string? filterByUsergroupName = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<InformationBarrierListResponse>("admin.barriers.list", new Args
            {
                { "filter_by_usergroup_name", filterByUsergroupName },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);

    public async Task<InformationBarrier> Update(
        string barrierId,
        string primaryUsergroupId,
        IEnumerable<string> barrieredFromUsergroupIds,
        IEnumerable<RestrictedSubject> restrictedSubjects,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Get<InformationBarrierResponse>("admin.barriers.update", new Args
            {
                { "barrier_id", barrierId },
                { "primary_usergroup_id", primaryUsergroupId },
                { "barriered_from_usergroup_ids", barrieredFromUsergroupIds },
                { "restricted_subjects", restrictedSubjects }
            }, cancellationToken).ConfigureAwait(false)).Barrier;
}
