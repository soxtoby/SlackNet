namespace SlackNet.WebApi;

public class ConversationOpenResponse : SlackObject
{
    public bool NoOp { get; set; }
    public bool AlreadyOpen { get; set; }
    public Conversation Channel { get; set; }
}