namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when the email domain settings for a team change.
    /// </summary>
    public class EmailDomainChanged : Event
    {
        public string EmailDomain { get; set; }
        public string EventTs { get; set; }
    }
}