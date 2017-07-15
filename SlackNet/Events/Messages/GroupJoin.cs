namespace SlackNet.Events
{
    /// <summary>
    /// Sent when a team member joins a private group.
    /// </summary>
    public class GroupJoin : MessageEvent
    {
        /// <summary>
        /// If the user was invited, the user ID of the inviting user.
        /// </summary>
        public string Inviter { get; set; }
    }
}