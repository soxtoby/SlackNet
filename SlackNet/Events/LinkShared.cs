using System.Collections.Generic;

namespace SlackNet.Events
{
    /// <summary>
    /// A message was posted containing one or more links relevant to your application.
    /// </summary>
    public class LinkShared : Event
    {
        public string Channel { get; set; }
        public string User { get; set; }
        public string MessageTs { get; set; }
        public IList<SharedLink> Links { get; set; } = new List<SharedLink>();
    }

    public class SharedLink
    {
        public string Domain { get; set; }
        public string Url { get; set; }
    }
}