namespace SlackNet.WebApi;

public class FileSearchResponse
{
    public string Query { get; set; }
    public SearchResults<File> Files { get; set; }
}