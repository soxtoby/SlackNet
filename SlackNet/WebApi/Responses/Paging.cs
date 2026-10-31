namespace SlackNet.WebApi;

public class Paging : SlackObject
{
    public int Count { get; set; }
    public int Total { get; set; }
    public int Page { get; set; }
    public int Pages { get; set; }
}