using SlackNet.WebApi;

namespace SlackNet.Interaction;

public class SlashCommandResponse : SlackObject, IMessageResponse
{
    public Message Message { get; set; }
    public ResponseType ResponseType { get; set; } = ResponseType.Ephemeral;
}