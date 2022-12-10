using SlackNet.Events;

namespace SlackNet.WebApi;

public class MessageSearchResponse
{
    public string Query { get; set; }
    public SearchResults<MessageEvent> Messages { get; set; }
}