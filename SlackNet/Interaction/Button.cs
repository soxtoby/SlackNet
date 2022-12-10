namespace SlackNet.Interaction;

public class Button : ActionElement
{
    public Button() : base("button") { }

    public Style Style { get; set; }
    public string Url { get; set; }
}