namespace SlackNet.WebApi;

public class MessageUpdateResponse
{
    public string Channel { get; set; } = null!;
    public string Ts { get; set; } = null!;
    public string Text { get; set; } = null!;
}