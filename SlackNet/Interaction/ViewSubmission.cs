using SlackNet.WebApi;

namespace SlackNet.Interaction;

public class ViewSubmission : InteractionRequest
{
    /// <summary>
    /// The source view of the modal that the user submitted.
    /// </summary>
    public ViewInfo View { get; set; }

    /// <summary>
    /// A unique value which is optionally accepted in <see cref="IViewsApi"/> API calls.
    /// When provided to those APIs, the hash is validated such that only the most recent view can be updated.
    /// This should be used to ensure the correct view is being updated when updates are happening asynchronously.
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    /// Present when a comment is posted or edited in a Work Object flexpane, to allow the app to acknowledge the action via <see cref="IEntityApi.AcknowledgeCommentAction"/>.
    /// </summary>
    public string TriggerId { get; set; }
}