using System.Collections.Generic;

namespace SlackNet.Events
{
    /// <summary>
    /// API tokens for your app were revoked.
    /// </summary>
    public class TokensRevoked : Event
    {
        /// <summary>
        /// Type of revoked token -> list of user IDs.
        /// </summary>
        public Dictionary<string, IList<string>> Tokens { get; set; } = new();
    }
}