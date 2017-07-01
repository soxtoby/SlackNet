using SlackNet.Objects;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connected clients for a user when that user uploads a file to Slack.
    /// </summary>
    public class FileCreated : Event
    {
        public File File { get; set; }
    }
}