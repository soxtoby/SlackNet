namespace SlackNet.WebApi;

public class FileSearchResponse : SlackObject
{
    public string Query { get; set; }
    public SearchResults<File> Files { get; set; }
}