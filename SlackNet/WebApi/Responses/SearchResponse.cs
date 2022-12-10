using SlackNet.Events;

namespace SlackNet.WebApi;

public class SearchResponse
{
    public string Query { get; set; }
    public SearchResults<MessageEvent> Messages { get; set; }
    public SearchResults<File> Files { get; set; }
}