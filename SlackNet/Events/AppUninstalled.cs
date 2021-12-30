using System;

namespace SlackNet.Events
{
    /// <summary>
    /// Your Slack app was uninstalled.
    /// </summary>
    public class AppUninstalled : Event {
        public string Token { get; set; }
        public string TeamId { get; set; }
        public string ApiAppId { get; set; }
    }
}