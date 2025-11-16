using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IEntityApi
{
    /// <summary>
    /// Populate the flexpane with content.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/entity.presentDetails/">Slack documentation</a> for more information.</remarks>
    /// <param name="triggerId">The trigger ID from the entity_details_requested event.</param>
    /// <param name="entity">The Work Object entity metadata. Should be URL-encoded JSON.</param>
    /// <param name="userAuthRequired">Set to true to indicate that user authentication is required.</param>
    /// <param name="userAuthUrl">URL for user authentication.</param>
    /// <param name="error">Error object to display in the flexpane.</param>
    /// <param name="cancellationToken"></param>
    Task PresentDetails(
        string triggerId,
        WorkObjectEntity entity = null,
        bool userAuthRequired = false,
        string userAuthUrl = null,
        EntityError error = null,
        CancellationToken cancellationToken = default);
}

public class EntityApi(ISlackApiClient client) : IEntityApi
{
    public Task PresentDetails(
        string triggerId,
        WorkObjectEntity entity = null,
        bool userAuthRequired = false,
        string userAuthUrl = null,
        EntityError error = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("entity.presentDetails", new Args
            {
                { "trigger_id", triggerId },
                { "metadata", entity },
                { "user_auth_required", userAuthRequired },
                { "user_auth_url", userAuthUrl },
                { "error", error }
            }, cancellationToken);
}