namespace SlackNet.WebApi;

public class ConversationHistoryResponse : ConversationMessagesResponse
{
    public string Latest { get; set; }
    public int PinCount { get; set; }
}