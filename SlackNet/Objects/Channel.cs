namespace SlackNet.Objects
{
    public class Channel : Hub
    {
        public bool IsChannel { get; set; }
        /// <summary>
        /// True if this channel is the "general" channel that includes all regular team members.
        /// </summary>
        public bool IsGeneral { get; set; }
        /// <summary>
        /// True if the calling member is part of the channel.
        /// </summary>
        public bool IsMember { get; set; }
    }
}