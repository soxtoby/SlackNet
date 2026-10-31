namespace SlackNet.WebApi;

public class MessageSearchResponse : SlackObject
{
    public string Query { get; set; }
    public SearchResults<MessageSearchResult> Messages { get; set; }
}