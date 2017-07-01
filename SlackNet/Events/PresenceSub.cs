using System.Collections.Generic;

namespace SlackNet.Events
{
    /// <summary>
    /// Ask Slack's message server to subscribe you to presence events for the specified list of users.
    /// </summary>
    public class PresenceSub
    {
        public string Type => "presence_sub";
        /// <summary>
        /// The user IDs you want presence subscriptions for.
        /// </summary>
        public IList<string> Ids { get; set; }
    }
}