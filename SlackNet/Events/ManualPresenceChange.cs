using SlackNet.Objects;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a user when that user manually updates their presence.
    /// </summary>
    public class ManualPresenceChange : Event
    {
        public Presence Presence { get; set; }
    }
}