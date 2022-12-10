namespace SlackNet.Events;

/// <summary>
/// Sent to all connections for a team when a new emoji has been added to the team emoji list.
/// </summary>
[SlackType("add")]
public class EmojiAdded : EmojiChanged
{
    public string Name { get; set; }
    /// <summary>
    /// Either the URI to fetch the image from or an alias to an existing name as indicated by the alias: pseudo-protocol.
    /// </summary>
    public string Value { get; set; }
}