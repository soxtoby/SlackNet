namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all clients in a DM channel When bulk changes have occurred to that DM channel's history.
    /// </summary>
    public class ImHistoryChanged : Event
    {
        /// <summary>
        /// Id of latest changed message.
        /// </summary>
        public string Latest { get; set; }
        public string Ts { get; set; }
    }
}