using SlackNet.Objects;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all websocket connections and event subscriptions when users leave public or private channels.
    /// </summary>
    public class MemberLeftChannel : Event
    {
        public string User { get; set; }
        public string Channel { get; set; }
        public ChannelType ChannelType { get; set; }
    }
}