namespace SlackNet.WebApi;

public class MessageSearchResponse
{
    public string Query { get; set; }
    public SearchResults<MessageSearchResult> Messages { get; set; }
}