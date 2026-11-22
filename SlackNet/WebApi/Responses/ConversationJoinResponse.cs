namespace SlackNet.WebApi;

public class ConversationJoinResponse
{
    public Conversation Channel { get; set; } = null!;
    public string? Warning { get; set; }
    public ConversationJoinResponseMetadata? ResponseMetadata { get; set; }
}