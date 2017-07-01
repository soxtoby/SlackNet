using SlackNet.Objects;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a user when that user joins a channel.
    /// </summary>
    public class ChannelJoined : Event
    {
        public Channel Channel { get; set; }
    }
}