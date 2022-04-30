namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all event subscriptions when a new shared channel is created or a channel is converted into a shared channel.
    /// It's also triggered when an external workspace is added to an existing shared channel.
    /// </summary>
    public class ChannelShared : Event
    {
        /// <summary>
        /// The team ID of the workspace that has joined the channel. 
        /// </summary>
        public string ConnectedTeamId { get; set; }
        /// <summary>
        /// The ID for the public or private channel.
        /// </summary>
        public string Channel { get; set; }
    }
}