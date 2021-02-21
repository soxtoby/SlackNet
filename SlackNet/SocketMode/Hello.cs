using System;

namespace SlackNet.SocketMode
{
    public class Hello : SocketMessage
    {
        public HelloConnectionInfo ConnectionInfo { get; set; }
        public int NumConnections { get; set; }
        public HelloDebugInfo DebugInfo { get; set; }
    }

    public class HelloConnectionInfo
    {
        public string AppId { get; set; }
    }

    public class HelloDebugInfo
    {
        public string Host { get; set; }
        public DateTime Started { get; set; }
        public int BuildNumber { get; set; }
        /// <summary>
        /// Estimate of how long the connection will persist until Slack refreshes it.
        /// </summary>
        public int ApproximateConnectionTime { get; set; }
    }
}