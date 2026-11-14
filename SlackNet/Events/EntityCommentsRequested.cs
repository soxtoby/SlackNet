namespace SlackNet.Events;

/// <summary>
/// Sent when a user opens the comments tab of a Work Object flexpane, refreshes the comments, or requests the next page of comments.
/// Respond by calling <see cref="WebApi.IEntityApi.PresentComments"/> with the requested comments.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/entity_comments_requested/">Slack documentation</a> for more information.</remarks>
public class EntityCommentsRequested : Event
{
    /// <summary>
    /// User who opened the flexpane.
    /// </summary>
    public string User { get; set; }
    public ExternalReference ExternalRef { get; set; }
    public string EntityUrl { get; set; }
    public SharedLink Link { get; set; }

    /// <summary>
    /// The pagination cursor your app supplied in a previous response.
    /// When present, return the next page of comments starting from that cursor.
    /// </summary>
    public string Cursor { get; set; }
    public int Limit { get; set; }

    /// <summary>
    /// Present when the replies for a specific comment thread are being requested.
    /// When present, return the replies for that comment thread instead of top-level comments.
    /// </summary>
    public string ThreadRootId { get; set; }
    public string TriggerId { get; set; }
}
