#nullable disable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
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

    /// <summary>
    /// Provide comments for Work Objects. Apps call this method to send per-user flexpane comment data to the client.
    /// Call it in response to the <see cref="EntityCommentsRequested"/> event.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/entity.presentComments/">Slack documentation</a> for more information.</remarks>
    /// <param name="triggerId">A reference to the original user action that initiated the request.</param>
    /// <param name="comments">Comments to present to the user.</param>
    /// <param name="canPostComment">Indicates whether the user has permissions to post comments. Hides the composer if false.</param>
    /// <param name="cursor">App supplied cursor used for pagination, will be sent in the next request for comments.</param>
    /// <param name="deleteActionId">
    /// The block action ID that will be sent when a delete request is initiated for a comment.
    /// The comment's <see cref="EntityComment.Id"/> will be sent as the action's value.
    /// </param>
    /// <param name="userAuthRequired">Set to true to indicate that the user must authenticate to see the comments data.</param>
    /// <param name="userAuthUrl">A custom URL to which users are directed for authentication if required.</param>
    /// <param name="error">Error object to display in the flexpane.</param>
    /// <param name="cancellationToken"></param>
    Task PresentComments(
        string triggerId,
        IEnumerable<EntityComment> comments,
        bool canPostComment = false,
        string cursor = null,
        string deleteActionId = null,
        bool userAuthRequired = false,
        string userAuthUrl = null,
        EntityError error = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Acknowledge a comment post, edit, or delete on a Work Object. Apps call this method to confirm they have processed a comment action.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/entity.acknowledgeCommentAction/">Slack documentation</a> for more information.</remarks>
    /// <param name="triggerId">A reference to the original user action that initiated the comment mutation.</param>
    /// <param name="comment">The full comment data. Required for edit and post actions.</param>
    /// <param name="error">Error message if the action failed in the app. When present, signals that the mutation could not be completed.</param>
    /// <param name="cancellationToken"></param>
    Task AcknowledgeCommentAction(
        string triggerId,
        EntityComment comment = null,
        string error = null,
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

    public Task PresentComments(
        string triggerId,
        IEnumerable<EntityComment> comments,
        bool canPostComment = false,
        string cursor = null,
        string deleteActionId = null,
        bool userAuthRequired = false,
        string userAuthUrl = null,
        EntityError error = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("entity.presentComments", new Args
            {
                { "trigger_id", triggerId },
                { "comments", comments },
                { "can_post_comment", canPostComment },
                { "cursor", cursor },
                { "delete_action_id", deleteActionId },
                { "user_auth_required", userAuthRequired },
                { "user_auth_url", userAuthUrl },
                { "error", error }
            }, cancellationToken);

    public Task AcknowledgeCommentAction(
        string triggerId,
        EntityComment comment = null,
        string error = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("entity.acknowledgeCommentAction", new Args
            {
                { "trigger_id", triggerId },
                { "comment", comment },
                { "error", error }
            }, cancellationToken);
}
