using System.Collections.Generic;
using SlackNet.Events.Messages;

namespace SlackNet.WebApi.Responses
{
    public class ChannelHistoryResponse
    {
        public string Latest { get; set; }
        public IList<Message> Messages { get; set; } = new List<Message>();
        public bool HasMore { get; set; }
        public bool IsLimited { get; set; }
        public int UnreadCountDisplay { get; set; }
    }
}