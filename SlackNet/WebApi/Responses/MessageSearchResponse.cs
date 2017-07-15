using SlackNet.Events.Messages;

namespace SlackNet.WebApi.Responses
{
    public class MessageSearchResponse
    {
        public string Query { get; set; }
        public SearchResults<MessageEvent> Messages { get; set; }
    }
}