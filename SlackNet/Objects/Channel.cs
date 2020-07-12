using System;

namespace SlackNet
{
    [Obsolete("Use Conversation instead")]
    public class Channel : Hub
    {
        /// <summary>
        /// The name of the channel, without a leading hash sign.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The user ID of the member that created this channel.
        /// </summary>
        public string Creator { get; set; }
        public bool IsArchived { get; set; }
        public Topic Topic { get; set; }
        public Topic Purpose { get; set; }

        public override string ToString() => Link.Hub(Id).ToString();
    }
}