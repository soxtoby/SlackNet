namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a user when that user leaves a private channel.
    /// </summary>
    public class GroupLeft : Event
    {
        public string Channel { get; set; }
    }
}