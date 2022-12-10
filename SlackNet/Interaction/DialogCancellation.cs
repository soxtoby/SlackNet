namespace SlackNet.Interaction;

public class DialogCancellation : InteractionRequest
{
    public string CallbackId { get; set; }
    public string State { get; set; }
}