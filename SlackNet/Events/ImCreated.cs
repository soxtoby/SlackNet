using SlackNet.Objects;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a user when a new direct message channel is created that they are a member of.
    /// </summary>
    public class ImCreated : Event
    {
        public string User { get; set; }
        public Channel Channel { get; set; }
    }
}