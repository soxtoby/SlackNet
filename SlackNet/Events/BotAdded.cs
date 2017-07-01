using SlackNet.Objects;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when an integration "bot" is added.
    /// </summary>
    public class BotAdded : Event
    {
        public Bot Bot { get; set; }
    }
}