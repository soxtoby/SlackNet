namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when an integration "bot" is updated.
    /// </summary>
    public class BotChanged : Event
    {
        public BotInfo Bot { get; set; }
    }
}