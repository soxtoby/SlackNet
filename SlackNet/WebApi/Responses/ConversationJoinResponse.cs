namespace SlackNet.WebApi;

public class ConversationJoinResponse : SlackObject
{
    public Conversation Channel { get; set; }
    public string Warning { get; set; }
    public ConversationJoinResponseMetadata ResponseMetadata { get; set; }
}