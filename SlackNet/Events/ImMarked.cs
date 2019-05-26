namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all open connections for a user when that user moves the read cursor in a direct message channel.
    /// </summary>
    public class ImMarked : Event
    {
        public string Channel { get; set; }
        public string Ts { get; set; }
    }
}