namespace SlackNet.WebApi;

public class SearchResponse : SlackObject
{
    public string Query { get; set; }
    public SearchResults<MessageSearchResult> Messages { get; set; }
    public SearchResults<File> Files { get; set; }
}