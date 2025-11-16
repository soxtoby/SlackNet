namespace SlackNet;

public class EntityUser
{
    /// <summary>
    /// Use with a Slack user ID (e.g. "U123ABC456") if known.
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// Use with the user's display name if the Slack user ID is not available.
    /// </summary>
    public string Text { get; set; }
    /// <summary>
    /// A link to an external profile for the user.
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// The email of the user. When the email provided matches a valid Slack user, the Slack user will be displayed.
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// An avatar for the user.
    /// </summary>
    public EntityIcon Icon { get; set; }
}