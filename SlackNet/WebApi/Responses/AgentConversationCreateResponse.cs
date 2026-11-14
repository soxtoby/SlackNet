namespace SlackNet.WebApi;

record AgentConversationCreateResponse
{
    public string ChannelId { get; init; } = null!;
}
