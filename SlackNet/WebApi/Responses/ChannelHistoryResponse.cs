using System.Collections.Generic;
using SlackNet.Events;

namespace SlackNet.WebApi
{
    public class ChannelHistoryResponse
    {
        public string Latest { get; set; }
        public IList<MessageEvent> Messages { get; set; } = new List<MessageEvent>();
        public bool HasMore { get; set; }
        public bool IsLimited { get; set; }
        public int UnreadCountDisplay { get; set; }
    }
}