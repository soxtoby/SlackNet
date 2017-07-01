namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when a team channel is archived.
    /// </summary>
    public class ChannelArchive : Event
    {
        public string Channel { get; set; }
        public string User { get; set; }
    }
}