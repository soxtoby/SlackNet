namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all open connections for a user when that user moves the read cursor in a private channel.
    /// </summary>
    public class GroupMarked : Event
    {
        public string Channel { get; set; }
        public string Ts { get; set; }
    }
}