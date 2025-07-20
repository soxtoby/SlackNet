namespace SlackNet.Interaction;

public class Button() : ActionElement("button")
{
    public Style Style { get; set; }
    public string Url { get; set; }
}