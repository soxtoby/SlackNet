namespace SlackNet.WebApi;

public class SlackListResponse : SlackObject
{
    public string ListId { get; set; }
    public ListMetadata ListMetadata { get; set; }
}