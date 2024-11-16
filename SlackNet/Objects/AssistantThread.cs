namespace SlackNet;

public class AssistantThread
{
    public string UserId { get; set; }
    public AssistantThreadContext Context { get; set; } = new();
    public string ChannelId { get; set; }
    public string ThreadTs { get; set; }
}

public class AssistantThreadContext
{
    public string ChannelId { get; set; }
    public string TeamId { get; set; }
    public string EnterpriseId { get; set; }
}