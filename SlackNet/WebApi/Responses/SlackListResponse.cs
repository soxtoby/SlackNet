namespace SlackNet.WebApi;

public class SlackListResponse
{
    public string ListId { get; set; } = null!;
    public ListMetadata? ListMetadata { get; set; }
}