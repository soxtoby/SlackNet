namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when one or more emojis have been removed from the team emoji list.
    /// </summary>
    [SlackType("remove")]
    public class EmojiRemoved : EmojiChanged { }
}