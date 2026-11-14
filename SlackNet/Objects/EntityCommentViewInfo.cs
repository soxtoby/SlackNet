namespace SlackNet;

/// <summary>
/// The view submitted when a user posts or edits a comment in the comments tab of a Work Object flexpane.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/messaging/work-objects-comments/#post-edit-comment">Slack documentation</a> for more information.</remarks>
[SlackType("entity_comment")]
public class EntityCommentViewInfo : ViewInfo
{
    public ExternalReference ExternalRef { get; set; }
    public string EntityUrl { get; set; }
}
